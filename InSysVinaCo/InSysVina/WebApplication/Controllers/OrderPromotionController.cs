using LIB;
using LIB.Warehouses;
using LibCore.EF;
using LibCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Authorize;

namespace WebApplication.Controllers
{
    public class OrderPromotionController : BaseController
    {
        private readonly IOrderPromotion _orderPromotionService;
        private readonly IplOrder _order;
        private readonly IWarehouses _warehouseService;
        private readonly IOrderDetail _orderDetailService;
        public OrderPromotionController()
        {
            _orderPromotionService = SingletonIpl.GetInstance<IplOrderPromotion>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
            _order = SingletonIpl.GetInstance<IplOrder>();
            _orderDetailService = SingletonIpl.GetInstance<IplOrderDetail>();
        }
        // GET: OrderPromotion
        public ActionResult Index()
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            return View();
        }
        public JsonResult GetData(bootstrapTableParam obj, int? WarehouseId)
        {
            int totalRecord = 0;
            List<OrderEntity> datas = _orderPromotionService.GetData(obj, User.WarehouseId ?? WarehouseId, ref totalRecord);
            if (datas == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, data = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Create()
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            ViewBag.Action = 1;
            return View("CreateOrEdit",new OrderEntity());
        }
        public ActionResult Edit(int? Id)
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            ViewBag.Action = 2;
            OrderEntity Entity = _order.GetOrderById(Id.Value);
            return View("CreateOrEdit",Entity);
           
        }
        public ActionResult Detail(int? Id)
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            ViewBag.Action = 3;
            OrderEntity Entity = _order.GetOrderById(Id.Value);

            return View("CreateOrEdit", Entity);

        }
        [HttpPost]
        public ActionResult AutoCompletedPaidOrders(Select2Param param, int? WarehouseId)
        {
            try
            {
                WarehouseId = WarehouseId ?? User.WarehouseId;
                int totalRecord = 0;
                bootstrapTableParam obj = new bootstrapTableParam();
                obj.limit = 10;
                obj.offset = (param.page - 1) * 10;
                obj.search = param.term;
                List<OrderEntity> data = _orderPromotionService.GetPaidOrders(obj, WarehouseId, ref totalRecord);
                return Json(new { success = true, results = data, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Order }, ActionType = new ActionType[] { ActionType.View })]
        public JsonResult GetOrderDetailByOrderId(long? OrderId)
        {
            if (OrderId == null)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Istang = _order.GetOrderById(OrderId.Value).PromotedDate;
                List<OrderPromotionEntity> data = _orderPromotionService.GetAllByOrderId(OrderId.Value);
                List<OrderDetailEntity> datasellor = _orderDetailService.GetAllByOrderId(OrderId.Value);
                if (data == null || datasellor == null)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
                else return Json(new { success = true, datas = data, datasell = datasellor, isTang = Istang, total = data.Count }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdatePromotionOrder(long Id, List<OrderPromotionEntity> listDetail, byte isTang)
        {
            string message = "";
            bool kq = _orderPromotionService.UpdatePromotionOrder(Id, listDetail, isTang, ref message);
            return Json(new { success = kq, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(int? Id)
        {
            if (Id != null)
            {
                return Json(new { success = _orderPromotionService.DeletePromotionOrder(Id.Value) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}