using Dapper;
using Framework.EF;
using LIB.API;
using LIB.Orders;
using LIB.Report;
using LIB.Warehouses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LIB
{
    public class IplOrder : BaseService<OrderEntity, int>, IOrder
    {
        private readonly IAPI _apiService;
        private readonly ICustomer _customerService;
        private readonly IOrderPromotion _orderPromotionService;
        private readonly IWarehouses _warehousesrService;
        public IplOrder()
        {

            _apiService = SingletonIpl.GetInstance<IplAPI>();
            _customerService = SingletonIpl.GetInstance<IplCustomer>();
            _orderPromotionService = SingletonIpl.GetInstance<IplOrderPromotion>();
            _warehousesrService = SingletonIpl.GetInstance<IplWarehouses>();
        }
        public List<OrderEntity> GetDataOrders(bootstrapTableParam obj, int? WarehouseId, byte getGuest, ref int totalRecord)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@WarehouseId", WarehouseId);
                param.Add("@getGuest", getGuest);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var data = unitOfWork.Procedure<OrderEntity>("sp_Orders_GetData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public bool SaveOrder(OrderSubmitModel orderRequest, int UserId, ref long OrderId, ref string message)
        {
            try
            {
                decimal totalPrice = 0;
                OrderEntity order = orderRequest.Order;

                List<OrderDetailEntity> listOrderDetail = orderRequest.OrderDetail;
                //
                if (listOrderDetail.Count() > 0)
                {
                    if (order.Id == null)
                    {
                        order.CreatedBy = UserId;
                    }
                    else
                    {
                        order.ModifiedBy = UserId;
                    }
                    //tính thành tiền sản phẩm
                    totalPrice = listOrderDetail.Sum(t => t.Quantity * t.SellPrice - t.Quantity * t.SellPrice * t.Discount / 100);
                    order.ProductTotal = totalPrice;

                    //Check lại mã giảm giá
                    decimal discount = 0;
                    if ((order.CouponCode != null && order.CouponType != null) || order.CouponValue != null)
                    {
                        DiscountResult discountValue = new DiscountResult();
                        DiscountModel discountObj = new DiscountModel();
                        order.CouponCode = order.CouponCode.Trim();
                        int check = _apiService.API_Momart_GetDiscount(order.CouponCode, totalPrice, ref discountValue, ref discountObj);
                        if (check != 4)
                        {
                            order.CouponCode = null;
                            order.CouponType = null;
                            order.CouponValue = null;
                        }
                        else
                        {
                            order.CouponType = discountValue.type;
                            order.CouponValue = discountValue.DiscountValue;
                            if (discountValue.type == 1 && discountValue.DiscountValue != null)
                            {
                                discount = discountValue.DiscountValue.Value;
                                if (discountObj.MaxValue != null && discountValue.DiscountValue > discountObj.MaxValue)
                                {
                                    discount = discountObj.MaxValue.Value;
                                }
                            }
                            else
                            {
                                discount = totalPrice * (discountValue.DiscountValue ?? 0) / 100;
                                if (discountObj.MaxValue != null && discount > discountObj.MaxValue)
                                {
                                    discount = discountObj.MaxValue.Value;
                                }
                            }
                            totalPrice = totalPrice - discount;
                        }
                    }
                    else
                    {
                        order.CouponCode = null;
                        order.CouponType = null;
                        order.CouponValue = null;
                        //voucher
                        if (order.Voucher != null)
                        {
                            discount = order.Voucher.Value;
                            if (discount > totalPrice)
                            {
                                discount = totalPrice;
                                totalPrice = 0;
                            }
                            else
                            {
                                totalPrice = totalPrice - discount;
                            }
                        }
                    }
                    order.Discount = discount;
                    var totalBeforUsePoint = totalPrice;
                    //sử dụng điểm point
                    if (order.CustomerId != null && order.PointUsed != null && totalPrice > 0)
                    {
                        //lấy tổng tiền điểm của khách hàng
                        CustomerEntity customer = _customerService.GetByID(order.CustomerId.Value);
                        if (customer.Point > 0)
                        {
                            decimal MoneyOnePoint = decimal.Parse(ConfigurationManager.AppSettings["MoneyOnePoint"]);
                            var moneyAllPoint = order.PointUsed.Value * MoneyOnePoint;

                            order.MoneyOnePoint = MoneyOnePoint;
                            if (moneyAllPoint > totalPrice)
                            {
                                order.PointUsed = (int)(totalPrice / MoneyOnePoint);
                                totalPrice = 0;
                            }
                            else
                            {
                                //order.PointUsed = customer.Point;
                                totalPrice = totalPrice - moneyAllPoint;
                            }
                        }
                    }
                    else
                    {
                        order.PointUsed = 0;
                    }
                    order.GrandTotal = totalPrice;
                    order.RefundMoney = (order.PayCash ?? 0) + (order.PayByCard ?? 0) - totalPrice;
                    if (order.RefundMoney < 0 && order.Status == 1)
                    {
                        message = "Khách đưa thiếu tiền!";
                        return false;
                    }
                    else
                    {
                        // tích điểm
                        double savepoint = 0;
                        if (orderRequest.Order.CustomerId != null && order.Voucher == null && discount == 0)
                        {
                            float PercentForPoint = float.Parse(ConfigurationManager.AppSettings["PercentForPoint"]);
                            order.PercentForPoint = PercentForPoint;
                            

                            savepoint = AccumulationPoint(totalPrice);

                        }

                        //Tính tiền giảm khi sử dụng điểm
                        var xmlOrder = XMLHelper.SerializeXML<OrderEntity>(order).Replace("xsi:nil=\"true\"", "");

                        var listProductPromotion = orderRequest.OrderPromotion;

                        var xmlOrderDetailSell = listOrderDetail.Count == 0 ? null : XMLHelper.SerializeXML<List<OrderDetailEntity>>(listOrderDetail).Replace("xsi:nil=\"true\"", "");
                        var xmlOrderDetailPromotion = (listProductPromotion == null || listProductPromotion.Count == 0) ? null : XMLHelper.SerializeXML<List<OrderPromotionEntity>>(listProductPromotion).Replace("xsi:nil=\"true\"", "");
                        var Prefix = "";
                        var Warehouse = _warehousesrService.GetById(int.Parse(order.WarehouseId.ToString())).Prefix;

                        if (Warehouse == null || Warehouse == "")
                        {
                            Prefix = Enum.PrefixCode.Order;
                        }
                        else
                        { Prefix = Warehouse; }
                        long validate = 1;
                        var param = new DynamicParameters();
                        param.Add("@OrderId", order.Id, DbType.Int64, ParameterDirection.InputOutput);
                        param.Add("@CustomerId", order.CustomerId);
                        param.Add("@CouponCode", order.CouponCode);
                        param.Add("@CouponType", order.CouponType);
                        param.Add("@CouponValue", order.CouponValue);
                        param.Add("@Voucher", order.Voucher);
                        param.Add("@Discount", order.Discount);
                        param.Add("@PercentForPoint", order.PercentForPoint);
                        param.Add("@MoneyOnePoint", order.MoneyOnePoint);
                        param.Add("@PointUsed", order.PointUsed);
                        param.Add("@ProductTotal", order.ProductTotal);
                        param.Add("@GrandTotal", order.GrandTotal);
                        param.Add("@WarehouseId", order.WarehouseId);
                        param.Add("@PayCash", order.PayCash);
                        param.Add("@PayByCard", order.PayByCard);
                        param.Add("@RefundMoney", order.RefundMoney);
                        param.Add("@Note", order.Note);
                        param.Add("@Status", order.Status);
                        param.Add("@CreatedBy", order.CreatedBy);
                        param.Add("@ModifiedBy", order.ModifiedBy);
                        param.Add("@pointSave", savepoint);
                        param.Add("@xmlOrderDetailSell", xmlOrderDetailSell);
                        param.Add("@xmlOrderDetailPromotion", xmlOrderDetailPromotion);
                        param.Add("@Prefix", Prefix);
                        param.Add("@Validate", validate, DbType.Int64, ParameterDirection.InputOutput);
                        bool kq = unitOfWork.ProcedureExecute("sp_Orders_CreateOrUpdate", param);
                        OrderId = param.Get<long>("@OrderId");
                        validate = param.Get<long>("@Validate");
                        if (validate > 0)
                        {
                            message = "Có sản phẩm không đủ số lượng trong lúc đặt hàng";
                            return false;
                        }
                        return kq;
                    }
                }
                else
                {
                    message = "Chưa chọn sản phẩm!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public double AccumulationPoint(decimal totalPrice)
        {
            if (totalPrice >= decimal.Parse(ConfigurationManager.AppSettings["MinMoneyUseSavePoint"]))
            {
                int day = DateTime.Now.Day;
                int dayx2 = int.Parse(ConfigurationManager.AppSettings["Dayx2"]);
                double point;
                //nhân đôi điểm tích--
                if (day == dayx2)
                {
                    point = (double.Parse(totalPrice.ToString()) * double.Parse(ConfigurationManager.AppSettings["PercentForPoint"].ToString()) / 100000) * 2;
                }
                else
                {
                    point = double.Parse(totalPrice.ToString()) * double.Parse(ConfigurationManager.AppSettings["PercentForPoint"].ToString()) / 100000;
                }
                double intdigits = Math.Truncate(point);
                double floatdigits = point - intdigits;
                if (floatdigits <= 0.5) point = intdigits;
                else point = intdigits + 1;
                return point;
            }
            else return 0;
        }
        public OrderEntity GetOrderById(long OrderId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", OrderId);
                return unitOfWork.Procedure<OrderEntity>("sp_Orders_GetById", param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public bool DeleteOrder(long OrderId)
        {
            List<ProductSyncModel> list;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                list = unitOfWork.Procedure<ProductSyncModel>("sp_Orders_Detete", param).ToList();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public List<OrderReportModel> GetDataOrdersReport(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, int? WarhouseId, long? CustomerSearchId, int? CreatedSearchId, byte opPayCash, byte opPayByCard, ref int totalRecord, ref decimal totalPrice)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@opType", opType);
                param.Add("@startDate", startDate);
                param.Add("@endDate", endDate);
                param.Add("@WarehouseId", WarhouseId);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@CustomerSearchId", CustomerSearchId);
                param.Add("@CreatedSearchId", CreatedSearchId);
                param.Add("@opPayCash", opPayCash);
                param.Add("@opPayByCard", opPayByCard);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@totalPrice", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                var data = unitOfWork.Procedure<OrderReportModel>("sp_Orders_Report", param).ToList();
                totalRecord = param.Get<Int32>("@totalRecord");
                totalPrice = param.Get<decimal>("@totalPrice");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<DetailGeneralModel> GetDataDetailGeneral(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, int? WarhouseId, int? CreatedSearchId, int? CustomerSearchId, ref int totalRecord)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@opType", opType);
                param.Add("@startDate", startDate);
                param.Add("@endDate", endDate);
                param.Add("@WarehouseId", WarhouseId);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@CreatedSearchId", CreatedSearchId);
                param.Add("@CustomerSearchId", CustomerSearchId);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var data = unitOfWork.Procedure<DetailGeneralModel>("sp_Report_DetailGeneral", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<OrderDetailEntity> GetDataOrderDetailByOrderId(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<OrderDetailEntity>("sp_Orders_GetDataDetail", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public CustomerEntity GetCustomerByOrderId(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<CustomerEntity>("sp_Orders_GetCustomerByOrderId", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public BillModel GetDataBill(long OrderId)
        {
            try
            {
                BillModel bill = new BillModel();
                bill.Order = GetOrderById(OrderId);
                bill.listOrderDetail = GetDataOrderDetailByOrderId(OrderId);
                bill.listProductPromotion = _orderPromotionService.GetAllByOrderId(OrderId);
                if (bill.Order.CustomerId != null)
                {
                    bill.Customer = GetCustomerByOrderId(OrderId);
                }
                return bill;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public decimal GetQuotaPromotion(int WarehouseId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@WarehouseId", WarehouseId);
                return unitOfWork.Procedure<decimal>("sp_Warehouse_GetQuotaPromotion", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return 0;
            }
        }
        public List<Orders_GetRevenueModel> GetRevenue(int opSearch, int IDWarehouse)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@opSearch", opSearch);
                param.Add("@IDWarehouse", IDWarehouse);
                return unitOfWork.Procedure<Orders_GetRevenueModel>("sp_Orders_GetRevenue", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public string GetOrderSubmit(int PageIndex, int PageSize, int IdApi, int IdWarehouse, ref int TotalRows)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageIndex", PageIndex);
                param.Add("@PageSize", PageSize);
                param.Add("@IdApi", IdApi);
                if (IdWarehouse != 0) param.Add("@IdWarehouse", IdWarehouse);
                param.Add("@Xml", dbType: DbType.Xml, direction: ParameterDirection.Output, size: 9999);
                param.Add("@TotalRows", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 9999);
                int result = IdWarehouse != 0 ? unitOfWork.Procedure<int>("API_Orders_GetOrderSubmit_byWarehouse", param).FirstOrDefault() : unitOfWork.Procedure<int>("Job_Orders_GetOrderSubmit", param).FirstOrDefault();

                var xml = param.Get<string>("@Xml");
                TotalRows = param.Get<Int32>("@TotalRows");
                return xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int SaveOrderResponse(string xml)
        {
            try
            {
                const string procedure = "Jod_Orders_SaveOrderResponse";
                var param = new DynamicParameters();
                param.Add("@xml", xml);
                var result = unitOfWork.Procedure<int>(procedure, param).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
