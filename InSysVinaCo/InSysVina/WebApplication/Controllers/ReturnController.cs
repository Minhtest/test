using Framework.EF;
using LIB;
using LIB.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Authorize;
namespace WebApplication.Controllers
{
    public class ReturnController : BaseController
    {
        private readonly IReturn _returnService;
        private readonly IWarehouses _warehouseService;
        public ReturnController()
        {
            _returnService = SingletonIpl.GetInstance<IplReturn>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
        }
        // GET: Return
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Return }, ActionType = new ActionType[] { ActionType.View })]
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
            WarehouseId = WarehouseId ?? User.WarehouseId;
            int total = 0;
            var list = _returnService.GetData(obj, WarehouseId, ref total);
            if (User.IsStaff)
            {
                list.All(t => { t.IsDel = 0; return true; });
            }
            return Json(new { datas = _returnService.GetData(obj, WarehouseId, ref total), total }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            ReturnEntity returnEntity = new ReturnEntity();
            return View("CreateOrEdit", returnEntity);
        }
        public ActionResult Edit(int Id)
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAllData();
            }
            ReturnEntity returnEntity = GetById(Id);
            if (returnEntity == null)
            {
                return Redirect("/Return/Create");
            }
            else
            {
                return View("CreateOrEdit", returnEntity);
            }
        }
        public ActionResult Detail(int? Id)
        {
            if (User.IsSupperAdmin)
            {
                ViewBag.AllWarehouse = _warehouseService.GetAll();
            }
            ReturnEntity returnEntity = GetById(Id.Value);
            return View(returnEntity);
        }
        public ReturnEntity GetById(int Id)
        {
            return _returnService.GetReturnById(Id);
        }
        public JsonResult GetDataById(int Id)
        {
            List<ReturnEntity> list = new List<ReturnEntity>();
            ReturnEntity item = GetById(Id);
            if (item != null)
            {
                list.Add(item);
            }
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertOrUpdate(int? Id, long OrderId, int? WarehouseId, List<ReturnDetailEntity> listDetail)
        {
            WarehouseId = WarehouseId ?? User.WarehouseId;
            return Json(new { success = _returnService.InsertOrUpdate(Id, OrderId, User.Id, WarehouseId.Value, listDetail) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReturnDetailByOrderId(int? ReturnId, int OrderId, int? WarehouseId)
        {
            WarehouseId = WarehouseId ?? User.WarehouseId;
            ReturnEntity returnEntity = _returnService.GetByOrderId_WarehouseId(OrderId, WarehouseId.Value);
            List<ReturnDetailEntity> data = _returnService.GetReturnDetailByOrderId(OrderId);
            if (data == null)
            {
                //fdf
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { success = true, ReturnId, returnEntity, data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReturnDetail(int ReturnId)
        {
            List<ReturnDetailEntity> data = _returnService.GetReturnDetail(ReturnId);
            if (data == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { success = true, total = data.Count(), data }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AutoCompletedOrder(Select2Param param, int? WarehouseId)
        {
            try
            {
                WarehouseId = WarehouseId ?? User.WarehouseId;
                int totalRecord = 0;
                bootstrapTableParam obj = new bootstrapTableParam();
                obj.limit = 10;
                obj.offset = (param.page - 1) * 10;
                obj.search = param.term;
                List<OrderEntity> data = _returnService.GetDataOrders(obj, WarehouseId, ref totalRecord);
                return Json(new { success = true, results = data, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int? Id)
        {
            if (Id != null)
            {
                return Json(new { success = _returnService.Delete(Id.Value) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}