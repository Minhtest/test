using Framework.EF;
using LIB;
using LIB.API;
using LIB.SyncSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Authorize;

namespace WebApplication.Controllers
{
    public class SyncSettingController : BaseController
    {
        private readonly IApiConnect _ApiConnectService;
        private readonly IAPI _apiService;
        public SyncSettingController()
        {
            _ApiConnectService = SingletonIpl.GetInstance<IplApiConnect>();
            _apiService = SingletonIpl.GetInstance<IplAPI>();
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetData(bootstrapTableParam obj)
        {
            try
            {
                int totalRecord = 0;
                var datas = _ApiConnectService.GetData(obj, ref totalRecord);
                return Json(new { success = true, datas = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetSelect2Data()
        {
            try
            {
                var datas = _ApiConnectService.GetSelect2Data();
                return Json(new { success = true, datas = datas }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.Add })]
        public ActionResult Create()
        {
            return View("Edit", new ApiConnectEntity());
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.Edit })]
        public ActionResult Edit(int Id)
        {
            ApiConnectEntity apiConnect = _ApiConnectService.GetDetail(Id);
            return View(apiConnect);
        }


        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.Edit })]
        public JsonResult UpdateApiConnect(ApiConnectEntity model)
        {
            return Json(new { success = _ApiConnectService.UpdateApiConnect(model), message = model.Id != 0 ? "Cập nhật" : "Thêm mới" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.Delete })]
        public ActionResult DeleteApiConnect(int id)
        {
            bool deleteResponse = _ApiConnectService.DeleteApiConnect(id);
            return Json(new { success = deleteResponse }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[UserAuthorize(Modules = new ActionModule[] { ActionModule.SyncSetting }, ActionType = new ActionType[] { ActionType.Delete })]
        public ActionResult GetByWarehouseId(int id)
        {
            try
            {
                var datas = _ApiConnectService.GetByWarehouseId(id);
                return Json(new { success = true, datas = datas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}