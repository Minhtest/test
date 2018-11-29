using Framework.EF;
using LIB;
using LIB.Product;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Authorize;
using OfficeOpenXml;
using static LIB.ExcelExtension;
namespace WebApplication.Controllers
{
    public class CategoryController : BaseController
    {

        private ICategory _categoryService;
        private IProduct _productService;

        public CategoryController()
        {
            _categoryService = SingletonIpl.GetInstance<IplCategory>();
            _productService = SingletonIpl.GetInstance<IplProduct>();
        }
        public ActionResult Index()
        {
            List<CategoryEntity> list = _categoryService.GetAllWithLevel("");
            var data = getChildList(list, null, "");
            ViewBag.listCategory = data;
            return View();
        }
        public List<CategoryEntity> getChildList(List<CategoryEntity> data, int? IdParent, string space)
        {
            List<CategoryEntity> con = new List<CategoryEntity>();
            foreach (var itemParent in data)
            {
                if (itemParent.ParentId == IdParent)
                {
                    con.Add(itemParent);
                    data = data.Where(m => m.Id != itemParent.Id).ToList();
                    List<CategoryEntity> dataChild = getChildList(data, itemParent.Id.Value, space);
                    foreach (var itemChild in dataChild)
                    {
                        itemChild.Name = space + itemChild.Name;
                        con.Add(itemChild);
                    }
                }
            }
            return con;
        }

        public JsonResult GetAllWithLevel(string txtSearch)
        {
            try
            {
                List<CategoryEntity> data = _categoryService.GetAllWithLevel(txtSearch);
                return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetDataWithPage(Select2Param obj)
        {
            int total = 0;
            var data = _categoryService.GetDataWithPage(obj, ref total);
            return Json(new { success = true, results = data, total = total }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create()
        {
            List<CategoryEntity> list = _categoryService.GetAllWithLevel("");
            if (list == null)
            {
                ViewBag.Categories = new List<CategoryEntity>();
                return PartialView("_Create_Edit", new CategoryEntity());
            }
            else
            {
                var data = getChildList(list, null, "--- ");
                ViewBag.Categories = data;
                return PartialView("_Create_Edit", new CategoryEntity());
            }
        }
        public ActionResult Edit(int Id)
        {
            List<CategoryEntity> list = _categoryService.GetAllWithLevel("");
            if (list == null)
            {
                ViewBag.Categories = new List<CategoryEntity>();
                return PartialView("_Create_Edit", new CategoryEntity());
            }
            else
            {
                list = list.Where(t => t.Id != Id).ToList();
                var data = getChildList(list, null, "--- ");
                ViewBag.Categories = data;
                CategoryEntity category = _categoryService.GetByID(Id);
                return PartialView("_Create_Edit", category);
            }
        }
        public JsonResult Insert_Update(CategoryEntity category)
        {
            try
            {
                if (category.Code != "")
                {
                    if (_categoryService.CheckExistCode(category.Id, category.Code))
                    {
                        return Json(new { warning = true, message = "Mã danh mục đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = _categoryService.Insert_Update(category) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult Delete(int Id)
        {
            try
            {
                return Json(new { success = _categoryService.DeleteById(Id) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        // [UserAuthorize(Modules = new ActionModule[] { ActionModule.Category }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult ReadFileImportExcel(HttpPostedFileBase fileImportExcel)
        {
            try
            {
                if (fileImportExcel == null)
                {
                    return Json(new { success = false, message = "Tập tin không tồn tại!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //save file
                    string pathSave = Server.MapPath(ConfigurationManager.AppSettings["PathUploadFileImport"]);
                    fileImportExcel.SaveAs(pathSave + fileImportExcel.FileName);
                    //read file to list model
                    List<string> listError = new List<string>();
                    List<CategoryEntity> list = ReadtoList<CategoryEntity>(pathSave + fileImportExcel.FileName, ref listError).Select(t => { t.ImportSuccess = ""; return t; }).ToList();
                    foreach (var item in list)
                    {
                        if (item.Name == null || item.Code == null)
                        {
                            item.ImportMessage += "Tên danh mục hoặc mã danh mục không được để trống";
                        }
                        else
                        {
                            if (_categoryService.CheckExistCate(item.Name))
                            {
                                item.ImportMessage += "Tên danh mục đã tồn tại";
                            }
                            if (_categoryService.CheckExistCate(item.Code))
                            {
                                item.ImportMessage += "Mã danh mục đã tồn tại";
                            }
                        }
                    }


                    return Json(new { success = true, lstCate = list, listError }, JsonRequestBehavior.AllowGet);
                    //return UpdateImportExcel(list, listError);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        //[UserAuthorize(Modules = new ActionModule[] { ActionModule.Category }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult UpdateImportExcel(List<CategoryEntity> data, List<string> listError)
        {
            if (data.Count > 0)
            {
                data.ForEach(t =>
                {
                    if (t.Name == "" || t.Name == null || t.Code == "" || t.Code == null)
                    {
                        t.ImportSuccess = "Không để trống Tên và Mã danh mục";
                    }
                    else
                    {
                        if (!_categoryService.CheckExistCode(t.Id, t.Code))
                        {
                            object status = "";
                            var cate = _categoryService.Insert_Update(t);
                            if (cate == true) t.ImportSuccess = "1";
                            else t.ImportSuccess = "0";
                        }
                        else
                        {
                            t.ImportSuccess = "Đã tồn tại";
                        }
                    }

                });
            }
            return Json(new { success = true, lstCate = data, listError  }, JsonRequestBehavior.AllowGet);
        }
    }
}