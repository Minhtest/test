using Framework.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LIB;
using WebApplication.Authorize;
using LIB.Orders;
using LIB.Product;
using LibCore.Helpers;
using System.Configuration;
using LIB.Warehouses;
using LIB.API;
using LIB.SyncSetting;
using LibCore.Helper;

namespace WebApplication.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IProduct _productService;
        private readonly IOrder _orderService;
        private readonly IOrderDetail _orderDetailService;
        private readonly IOrderPromotion _orderPromotionService;
        private readonly ICustomer _customerService;
        private readonly IAPI _apiService;
        private readonly IWarehouses _warehouseService;
        private readonly IConfig _configService;
        private readonly IApiConnect _apiConnectService;

        public OrdersController()
        {
            _productService = SingletonIpl.GetInstance<IplProduct>();
            _orderService = SingletonIpl.GetInstance<IplOrder>();
            _orderDetailService = SingletonIpl.GetInstance<IplOrderDetail>();
            _customerService = SingletonIpl.GetInstance<IplCustomer>();
            _apiService = SingletonIpl.GetInstance<IplAPI>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
            _configService = SingletonIpl.GetInstance<IplConfig>();
            _orderPromotionService = SingletonIpl.GetInstance<IplOrderPromotion>();
            _apiConnectService = SingletonIpl.GetInstance<IplApiConnect>();
        }
        // GET: Orders
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult Index()
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            return View("Index");
        }
        //GetDataOders
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public JsonResult GetDataOders(bootstrapTableParam obj, int? WarehouseId, byte getGuest)
        {
            WarehouseId = WarehouseId ?? User.WarehouseId;
            int totalRecord = 0;
            List<OrderEntity> datas = _orderService.GetDataOrders(obj, WarehouseId, getGuest, ref totalRecord);
            if (datas == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, data = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
        }
        //Create Order
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Add })]
        public ActionResult Create()
        {
            var model = new OrderEntity();
            int? WarehouseId = User.WarehouseId;
            if (User.IsSupperAdmin)
            {
                List<WarehousesEntity> list = _warehouseService.GetAllData();
                ViewBag.AllWarehouse = list;
                WarehouseId = list[0].Id;
            }
            ViewBag.QuotaPromotion = _orderService.GetQuotaPromotion(WarehouseId.Value);
            return View("Create_Edit", model);
        }
        #region Update Order
        [HttpGet]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Edit })]
        public ActionResult Edit(long Id)
        {
            OrderEntity model = _orderService.GetOrderById(Id);
            int? WarehouseId = User.WarehouseId;
            if (model != null)
            {
                if (User.IsSupperAdmin)
                {
                    List<WarehousesEntity> list = _warehouseService.GetAllData();
                    ViewBag.AllWarehouse = list;
                    WarehouseId = list[0].Id;
                }
                ViewBag.QuotaPromotion = _orderService.GetQuotaPromotion(WarehouseId.Value);
                return View("Create_Edit", model);
            }
            else
            {
                return RedirectToAction("Create");
            }
        }
        public decimal GetQuotaPromotion(int WarehouseId)
        {
            return _orderService.GetQuotaPromotion(WarehouseId);
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public JsonResult GetOrderDetailByOrderId(long OrderId)
        {
            List<OrderDetailEntity> data = _orderDetailService.GetAllByOrderId(OrderId);
            List<OrderPromotionEntity> orderPromotion = _orderPromotionService.GetAllByOrderId(OrderId);
            if (data == null)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, datas = data,dataspro = orderPromotion, total = data.Count }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public JsonResult GetProductPromotionByOrderId(long OrderId)
        {
            List<OrderPromotionEntity> data = _orderPromotionService.GetAllByOrderId(OrderId);
            if (data == null)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, datas = data, total = data.Count }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion update order

        #region Save order
        [HttpPost]
        public ActionResult SubmitOrder(OrderSubmitModel orderRequest)
        {
            if (orderRequest.Order.WarehouseId == null)
            {
                orderRequest.Order.WarehouseId = User.WarehouseId.Value;
            }
            if (orderRequest.Order.WarehouseId == null)
            {
                return Json(new { success = false, message = "Chưa có Showroom!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                long OrderID = 0;
                string message = "";
                if (_orderService.SaveOrder(orderRequest, User.Id, ref OrderID, ref message))
                {
                    return Json(new { success = true, Id = OrderID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
                }
            }

        }
        #endregion
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public JsonResult CheckCoupon(string couponCode, decimal TotalPrice)
        {
            DiscountResult discount = new DiscountResult();
            DiscountModel discountObj = new DiscountModel();
            int check = _apiService.API_Momart_GetDiscount(couponCode.Trim(), TotalPrice, ref discount, ref discountObj);
            return Json(new { checkValue = check, discount, discountObj }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult Detail(long Id)
        {
            OrderEntity order = _orderService.GetOrderById(Id);
            if (order != null)
            {
                ViewBag.order = order;
                if (order.CustomerId != null)
                {
                    CustomerEntity customer = _customerService.GetByID(order.CustomerId.Value);
                    ViewBag.customer = customer;
                }
                List<OrderDetailEntity> orderdetail = _orderDetailService.GetAllByOrderId(order.Id.Value);
                ViewBag.orderdetail = orderdetail;
                List<OrderPromotionEntity> orderPromotion = _orderPromotionService.GetAllByOrderId(order.Id.Value);
                ViewBag.orderPromotion = orderPromotion;
            }
            return View("Detail");
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Delete })]
        public JsonResult DeleteOrder(long Id)
        {
            return Json(new { kq = _orderService.DeleteOrder(Id) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintBillOrder(long Id)
        {
            BillModel bill = _orderService.GetDataBill(Id);
            if (bill == null)
            {
                return Redirect("/Error");
            }
            else
            {
                ViewBag.warehouse = _warehouseService.GetById(bill.Order.WarehouseId.Value);
                ViewBag.point = (bill.Order.CouponCode != null || (bill.Order.Voucher != null)) ? 0 : AccumulationPoint(bill.Order.GrandTotal);
                return PartialView("_ExportBill", bill);
            }
        }
        #region Report
        public ActionResult PrintOrderReport(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, string type, long? CustomerSearchId, int? CreatedSearchId, int? WarehouseId, byte opPayCash, byte opPayByCard)
        {
            try
            {
                ViewBag.opType = opType;
                if(opType == 0)
                {                   
                    ViewBag.TimeStart = String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date);
                    ViewBag.TimeEnd = String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date);
                }
                else if( opType == 1)
                {
                    ViewBag.TimeStart = String.Format("{0:dd/MM/yyyy}", FirstLastDayByCondition.FirstDayOfMonth_NewMethod(DateTime.Now));
                    ViewBag.TimeEnd = String.Format("{0:dd/MM/yyyy}", FirstLastDayByCondition.LastDayOfMonth_NewMethod(DateTime.Now));
                }
                else if(opType == 2)
                {
                    ViewBag.TimeStart = String.Format("{0:dd/MM/yyyy}", new DateTime(DateTime.Now.Year, 1, 1));
                    ViewBag.TimeEnd = String.Format("{0:dd/MM/yyyy}", new DateTime(DateTime.Now.Year, 12, 31));
                }
                else
                {
                    ViewBag.TimeStart = String.Format("{0:dd/MM/yyyy}", startDate??DateTime.Now);
                    ViewBag.TimeEnd = String.Format("{0:dd/MM/yyyy}", endDate??DateTime.Now);
                }
                ViewBag.warehouse = WarehouseId != null?_warehouseService.GetById(WarehouseId??0): User.WarehouseId != null? _warehouseService.GetById(User.WarehouseId??0) : new WarehousesEntity{ Name = "Tất cả", Address = "",Phone="" };
                int totalRecord = 0;
                decimal totalPrice = 0;
                List<OrderReportModel> datas = _orderService.GetDataOrdersReport(obj, opType, startDate, endDate, User.WarehouseId ?? WarehouseId, CustomerSearchId, CreatedSearchId, opPayCash, opPayByCard, ref totalRecord, ref totalPrice);
                var numReturn = datas.Sum(t => t.TotalPriceReturn ?? 0);
                ViewBag.Refund = String.Format("{0:0,0}", datas.Sum(t => t.RefundMoney));
                ViewBag.SumReturn = String.Format("{0:0,0}", numReturn);
                ViewBag.SumPayCard  = String.Format("{0:0,0}", datas.Sum(t => t.PayByCard??0));
                ViewBag.SumPayCash = String.Format("{0:0,0}", datas.Sum(t => t.PayCash??0));
                ViewBag.SumMoney = String.Format("{0:0,0}", datas.Sum(t => t.GrandTotal)- numReturn);
                return PartialView("_PrintReport", datas);
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Report })]
        public ActionResult Report()
        {
            ViewBag.AllWarehouse = _warehouseService.GetAllData();
            return View("Report");
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Report })]
        public JsonResult GetDataOrdersReport(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, string type, long? CustomerSearchId, int? CreatedSearchId, int? WarehouseId, byte opPayCash, byte opPayByCard)
        {
            try
            {
                int totalRecord = 0;
                decimal totalPrice = 0;
                List<OrderReportModel> datas = _orderService.GetDataOrdersReport(obj, opType, startDate, endDate, User.WarehouseId ?? WarehouseId, CustomerSearchId, CreatedSearchId, opPayCash, opPayByCard, ref totalRecord, ref totalPrice);
                return Json(new { success = true, data = datas, total = totalRecord, summary = totalPrice }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.Report })]
        public JsonResult ExportReport(string txtSearch, int opType, DateTime? startDate, DateTime? endDate, long? CustomerSearchId, int? CreatedSearchId, int? WarehouseId, byte opPayCash, byte opPayByCard)
        {
            try
            {
                WarehouseId = WarehouseId ?? User.WarehouseId;
                MemoryStream st = new MemoryStream();
                string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(ConfigurationManager.AppSettings["Template_Export_OrdersReport"]);
                using (ExcelTemplateHelper helper = new ExcelTemplateHelper(UrlTemplate, st))
                {
                    helper.Direction = ExcelTemplateHelper.DirectionType.TOP_TO_DOWN;
                    helper.CurrentSheetName = "Sheet2";
                    helper.TempSheetName = "Sheet1";
                    helper.CurrentPosition = new CellPosition("A1");

                    int totalRecord = 0;
                    bootstrapTableParam obj = new bootstrapTableParam();
                    obj.limit = 0;
                    obj.search = txtSearch;
                    decimal totalPrice = 0;
                    List<OrderReportModel> datas = _orderService.GetDataOrdersReport(obj, opType, startDate, endDate, WarehouseId, CustomerSearchId, CreatedSearchId, opPayCash, opPayByCard, ref totalRecord, ref totalPrice);
                    var sumCol = new OrderReportModel();
                    sumCol.OrderCode = "TỔNG";
                    sumCol.ProductTotal = datas.Sum(t => t.ProductTotal);
                    sumCol.Discount = datas.Sum(t => t.Discount??0);
                    sumCol.PointUsed = datas.Sum(t => t.PointUsed??0);
                    sumCol.GrandTotal = datas.Sum(t => t.GrandTotal);
                    sumCol.PayCash = datas.Sum(t => t.PayCash??0);
                    sumCol.PayByCard = datas.Sum(t => t.PayByCard??0);
                    sumCol.RefundMoney = datas.Sum(t => t.RefundMoney);
                    sumCol.TotalPriceReturn = datas.Sum(t => t.TotalPriceReturn??0);
                    datas.Add(sumCol);
                    var temp_head = helper.CreateTemplate("head");
                    var temp_body = helper.CreateTemplate("body");
                    
                    helper.Insert(temp_head);
                    helper.InsertDatas(temp_body, datas);
                    
                    helper.CopyWidth();
                }

                string fileName = "OrdersReport";
                if (opType == 0)
                {
                    fileName += "_" + DateTime.Now.ToString("dd-MM-yyyy");
                }
                else if (opType == 1)
                {
                    fileName += "_" + DateTime.Now.ToString("MM-yyyy");
                }
                else if (opType == 2)
                {
                    fileName += "_" + DateTime.Now.ToString("yyyy");
                }
                else if (opType == 3)
                {
                    if (startDate != null) fileName += "_" + startDate.Value.ToString("dd-MM-yyyy");
                    if (endDate != null) fileName += "_" + endDate.Value.ToString("dd-MM-yyyy");
                }
                else { }
                string pathFile = ConfigurationManager.AppSettings["PathSaveFileExport"] + fileName + ".xlsx";
                FileStream fileStream = new FileStream(Server.MapPath(pathFile), FileMode.Create, FileAccess.Write);
                st.WriteTo(fileStream);
                fileStream.Close();
                return Json(new { success = true, urlFile = pathFile, fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        #region Tính tích lũy
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
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
                //double point = double.Parse(totalPrice.ToString()) * double.Parse(ConfigurationManager.AppSettings["PercentForPoint"].ToString()) / 100000;
                double intdigits = Math.Truncate(point);
                double floatdigits = point - intdigits;
                if (floatdigits <= 0.5) point = intdigits;
                else point = intdigits + 1;
                return point;
            }
            else return 0;
        }
        #endregion
        #region hiện thông tin mã giảm giá
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult InfoCoupon(DiscountModel discountObj)
        {
            if (discountObj.Code == null)
            {
                return PartialView("_InfoCoupon", null);
            }
            else return PartialView("_InfoCoupon", discountObj);
        }
        #endregion
        [HttpPost]
        public ActionResult AutoCompletedOrder(Select2Param param)
        {
            try
            {
                int totalRecord = 0;
                bootstrapTableParam obj = new bootstrapTableParam();
                obj.limit = 10;
                obj.offset = (param.page - 1) * 10;
                obj.search = param.term;
                List<OrderEntity> data = _orderService.GetDataOrders(obj, User.WarehouseId, 0, ref totalRecord);
                return Json(new { success = true, results = data, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Sync(int? WarehouseId)

        {
            try
            {
                WarehouseId = WarehouseId ?? User.WarehouseId?? 0;

                return Json(_apiService.API_Warehouses_SyncDataProduct(WarehouseId.Value, _apiConnectService, _warehouseService, _orderService).Result);

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetNevenue(int opSearch, int? IDWarehouse)
        {
            List<Orders_GetRevenueModel> list = _orderService.GetRevenue(opSearch, IDWarehouse ?? User.WarehouseId ?? 0);
            if (list != null)
            {
                List<string> listLabel = list.Select(t => t.WarehouseName).ToList();
                List<decimal> listRevenue = list.Select(t => t.Revenue).ToList();
                return Json(new { success = true, listLabel, listRevenue }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}