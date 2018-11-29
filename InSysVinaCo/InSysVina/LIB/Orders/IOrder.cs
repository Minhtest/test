using LIB.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Orders
{
    public interface IOrder : IBaseServices<OrderEntity, long>
    {
        ///<summary>
        ///Lấy danh sách Hóa Đơn
        ///null: thất bại
        ///</summary>
        List<OrderEntity> GetDataOrders(bootstrapTableParam obj, int? WarehouseId, byte getGuest, ref int totalRecord);
        /// <summary>
        /// Lưu Hóa Đơn.
        /// false: thất bại
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        bool SaveOrder(OrderSubmitModel orderRequest, int UserId, ref long OrderId, ref string status);
        /// <summary>
        /// Lấy Hóa đơn theo ID
        /// | fales: Thất bại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OrderEntity GetOrderById(long OrderId);
        /// <summary>
        /// Xóa Order theo isActive
        /// 0: Xóa lỗi
        /// 1: xóa thành công, đồng bộ lỗi
        /// 2: thành công
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool DeleteOrder(long OrderId);
        /// <summary>
        /// lấy dữ liệu báo cáo hóa đơn
        /// null: Thất  bại
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="opType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="WarhouseId"></param>
        /// <param name="CustomerSearchId"></param>
        /// <param name="CreatedSearchId"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        List<OrderReportModel> GetDataOrdersReport(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, int? WarhouseId, long? CustomerSearchId, int? CreatedSearchId, byte opPayCash, byte opPayByCard, ref int totalRecord, ref decimal totalPrice);
        /// <summary>
        /// Lấy thông tin báo cáo tổng hợp
        /// null: thất bại
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="opType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="WarhouseId"></param>
        /// <param name="CreatedSearchId"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        List<DetailGeneralModel> GetDataDetailGeneral(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, int? WarhouseId, int? CreatedSearchId, int? CustomerSearchId, ref int totalRecord);
        /// <summary>
        /// lấy chi tiết order theo orderId
        /// null: Thất bại
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<OrderDetailEntity> GetDataOrderDetailByOrderId(long OrderId);
        /// <summary>
        /// lấy thông tin khách hàng theo order
        /// null: Thất bại
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        CustomerEntity GetCustomerByOrderId(long OrderId);
        /// <summary>
        /// lấy dữ liệu hóa đơn
        /// null: Thất bại
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        BillModel GetDataBill(long OrderId);
        decimal GetQuotaPromotion(int WarehouseId);
        List<Orders_GetRevenueModel> GetRevenue(int opSearch, int IDWarehouse);
        string GetOrderSubmit(int PageIndex, int PageSize, int IdApi, int IdWarehouse, ref int TotalRows);
        int SaveOrderResponse(string xml);
    }
}
