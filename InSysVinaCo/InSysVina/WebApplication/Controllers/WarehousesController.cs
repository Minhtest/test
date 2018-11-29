using Framework.EF;
using LIB;
using LIB.API;
using LIB.Model;
using LIB.SyncSetting;
using LIB.Warehouses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Authorize;

namespace WebApplication.Controllers
{
    public class WarehousesController : BaseController
    {
        private readonly IWarehouses _warehousesrService;
        private readonly IAPI _apiService;
        private readonly IApiConnect _apiConnectService;
        public WarehousesController()
        {
            _warehousesrService = SingletonIpl.GetInstance<IplWarehouses>();
            _apiService = SingletonIpl.GetInstance<IplAPI>();
            _apiConnectService = SingletonIpl.GetInstance<IplApiConnect>();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataWarehouses(bootstrapTableParam obj)
        {
            try
            {
                int totalRecord = 0;
                var datas = _warehousesrService.GetDataWarehouses(obj, ref totalRecord);
                return Json(new { success = true, datas = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Edit(int Id)
        {
            if ((User.WarehouseId != null && Id == User.WarehouseId) || User.WarehouseId == null)
            {
                WarehousesEntity warehouse = _warehousesrService.GetById(Id);
                ViewBag.ApiConnect = _apiConnectService.GetSelect2Data();
                return View(warehouse);
            }
            else return Redirect("/Unauthorised/Index");
        }

        [HttpPost]
        public JsonResult UpdateWarehouse(WarehousesEntity data, List<int?> ApisId)
        {
            return Json(new { success = _warehousesrService.UpdateWarehouse(data, string.Join(",", ApisId.ToArray())) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateLogoWarehouse(string base64Image, int? WarehouseId, string fileName)
        {
            string PathServer = ControllerContext.HttpContext.Server.MapPath("~");
            string PathFile = ConfigurationManager.AppSettings["PathUploadLogo"] + fileName;
            return Json(new { success = _warehousesrService.UpdateLogo(base64Image, User.WarehouseId == null ? WarehouseId.Value : User.WarehouseId.Value, PathServer, PathFile) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = _warehousesrService.GetById(id);
            entity.isActive = true;
            var deleteResponse = _warehousesrService.Update(entity);
            return JsonCamelCase(deleteResponse);
        }
        [HttpPost]
        public JsonResult Sync()
        {
            return Json(new { success = _apiService.API_Warehouses_SyncWarehouse(_apiConnectService) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int? WarehouseId)
        {
            WarehousesEntity warehouse = _warehousesrService.GetById(WarehouseId ?? User.WarehouseId.Value);
            return View(warehouse);
        }
        [HttpPost]
        public JsonResult CheckSyncWarehouse(int warehouseid)
        {
            WarehousesEntity warehouse = _warehousesrService.CheckSyncWarehouseId(warehouseid);
            if (warehouse != null)
            {
                return Json(new { data = true, name = warehouse.Name }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = false }, JsonRequestBehavior.AllowGet);
            }
           
        }
    }
}