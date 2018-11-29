using Framework.EF;
using LIB;
using LIB.Model;
using LIB.RoleModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApplication.Authorize;

namespace WebApplication.Controllers
{
    public class RoleModuleController : BaseController
    {

        private readonly IRight _rightService;
        private readonly IModule _moduleService;
        private readonly IRole _roleService;
        private readonly IplRoleModule _roleModule;
        private readonly IRoleMapRight _RoleMapRightService;
        // GET: Right
        public RoleModuleController()
        {
            _rightService = SingletonIpl.GetInstance<IplRight>();
            _moduleService = SingletonIpl.GetInstance<IplModule>();
            _roleService = SingletonIpl.GetInstance<IplRole>();
            _RoleMapRightService = SingletonIpl.GetInstance<IplRoleMapRight>();
            _roleModule = SingletonIpl.GetInstance<IplRoleModule>();
        }
    
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Role }, ActionType = new ActionType[] { ActionType.Report })]
        public ActionResult Index()
        {
            ViewBag.Roles = _roleService.GetRolesByLevel(User.RoleLevel);
            ViewBag.Modules = _moduleService.GetDataModule();
            return View("Index");
        }
        public ActionResult GetDataRoleModule(long RoleId)
        {
            List<RoleModuleEntity> role_module = _roleModule.GetDataRoleModule_ByRoleId(RoleId);
            return PartialView("_TableRoleModule", role_module);
        }
        public JsonResult Save(List<RoleModuleEntity> list)
        {
            try
            {
                string xml = XMLHelper.SerializeXML<List<RoleModuleEntity>>(list);
                bool kq = _roleModule.Save(xml);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Role }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult GetAllRight()
        {
            try
            {
                return JsonResultSuccess(_rightService.Raw_GetAll());
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Role }, ActionType = new ActionType[] { ActionType.Record })]
        public ActionResult SaveModuleAndOrder(List<RightEntity> datas)
        {
            try
            {
                var xml = XMLHelper.SerializeXML<List<RightEntity>>(datas);
                _rightService.UpdateModuleIdAndSort(xml);
                return JsonResultSuccess(true);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }
        [HttpPost]
        public JsonResult AddModuleToRole(int RoleId, List<int?> ModulesId)
        {
            return Json(new { success = _roleModule.AddModuleToRole(RoleId,String.Join(",", ModulesId.ToArray())) },JsonRequestBehavior.AllowGet);
        }

        #region Mpdule
        [HttpPost]
        public ActionResult ModuleSaveModuleAndOrder(List<ModuleEntity> datas)
        {
            try
            {
                var xml = XMLHelper.SerializeXML<List<ModuleEntity>>(datas);
                _moduleService.UpdateModuleIdAndSort(xml);
                return JsonResultSuccess(true);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        public ActionResult ModuleDelete(int Id)
        {
            try
            {
                return JsonResultSuccess(_moduleService.Raw_Delete(Id));
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }
        [HttpPost]
        public ActionResult ModuleCreate(ModuleEntity model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultError(ModelState);
                }
                var get = _moduleService.Raw_Get(model.Id);

                var columnIgnore = new List<string>() { "ModuleId", "Sorting" };

                if (get != null)
                {
                    _moduleService.Raw_Update(model, Columns: columnIgnore, IgnoreOrSave: true);
                }
                else
                {
                    model.Sorting = 1000;
                    _moduleService.Raw_Insert(model);
                }

                return JsonResultSuccess(true);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        public ActionResult ModuleGetListTreeView()
        {
            try
            {
                var list = _moduleService.ModuleGetListTreeView();
                //var list = _roleModule.GetAll().ToList();
                return JsonResultSuccess(list);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }
        #endregion

        #region Role
        [HttpGet]
        public ActionResult RoleList()
        {
            try
            {
                var list = _roleService.Raw_GetAll();
                //var list = _roleModule.GetAll().ToList();
                return JsonResultSuccess(list);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }
        [HttpPost]
        public ActionResult RoleCreate(RoleEntity model)
        {
            try
            {
                var get = _roleService.Raw_Get(model.Id);
                if (get == null)
                {
                    _roleService.Raw_Insert(model);
                }
                else
                {
                    _roleService.Raw_Update(model);
                }

                return JsonResultSuccess(1);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        public ActionResult RoleDelete(int Id)
        {
            try
            {
                return JsonResultSuccess(_roleService.Raw_Delete(Id));
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        public ActionResult RoleUpdateRight(int RoleId, List<RightEntity> datas)
        {
            try
            {
                //datas = datas ?? new List<RightEntity>() { new RightEntity() {
                //    Code="123",
                //    Id=0,
                //    Description="123",
                //    ModuleId=0,
                //    Name="123",
                //    Sorting=0
                //} };
                //var xml = XMLHelper.SerializeXML<List<RightEntity>>(datas);
                //_roleService.UpdateRight(RoleId, xml);
                datas = datas ?? new List<RightEntity>();
                var data_ = datas.Select(e =>
                {
                    return new RoleMapRightEntity()
                    {
                        RoleId = RoleId,
                        RightCode = e.Code
                    };
                });
                _RoleMapRightService.UpdateRoleMapRight(data_, RoleId);
                return JsonResultSuccess(true);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpPost]
        public ActionResult RoleGetRight(int RoleId)
        {
            try
            {
                return JsonResultSuccess(_roleService.GetRightByRole(RoleId));
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }

        [HttpGet]
        public ActionResult RoleManager()
        {
            return View("RoleManager");
        }
        [HttpPost]
        public ActionResult GetRoleModule(int roleId)
        {
            return JsonResultError("");
            //try
            //{
            //    var allRole = _roleModule.ListRoleModuleByRole(roleId);
            //    var list = allRole.Where(x => x.RoleId == roleId);
            //    return JsonResultSuccess(list.ToList());
            //}
            //catch (Exception ex)
            //{
            //    return JsonResultError(ex);
            //}
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Role }, ActionType = new ActionType[] { ActionType.Record })]
        public ActionResult EditRightModule(int roleId, int moduleId, bool status, string name)
        {
            try
            {
                var retval = _roleModule.UpdateRoleModule(roleId, moduleId, status, name);
                return JsonResultSuccess(null, "", retval);
            }
            catch (Exception ex)
            {
                return JsonResultError(ex);
            }
        }
        #endregion
    }
}