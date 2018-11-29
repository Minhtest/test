using Framework.EF;
using LIB;
using LIB.CardNumbers;
using LIB.Warehouses;
using LibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class CardnumbersController : BaseController
    {
        // GET: Cardnumbers
        private readonly ICardNumbers _cardnumbersServices;
        private readonly IWarehouses _warehouseService;
        public CardnumbersController()
        {
            _cardnumbersServices = SingletonIpl.GetInstance<IplCardNumbers>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
        }
        public ActionResult Index()
        {
            //ViewBag.AllWarehouse = _warehouseService.GetAll();
            return View();
        }
        public JsonResult GetAllData(bootstrapTableParam obj)
        {
            int totalRecord = 0;
            List<CardNumbersEntity> listCardnumber = _cardnumbersServices.GetAllData(obj, ref totalRecord);
            if (listCardnumber != null)
            {
                return Json(new { success = true, data = listCardnumber, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ExportReport(string txtSearch)
        {
            try
            {
                MemoryStream st = new MemoryStream();
                string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(@"~/Templates/Export/CardNumbersExport.xlsx");
                using (ExcelTemplateHelper helper = new ExcelTemplateHelper(UrlTemplate, st))
                {
                    helper.Direction = ExcelTemplateHelper.DirectionType.TOP_TO_DOWN;
                    helper.CurrentSheetName = "Sheet2";
                    helper.TempSheetName = "Sheet1";
                    helper.CurrentPosition = new CellPosition("A1");
                    bootstrapTableParam obj = new bootstrapTableParam();
                    obj.search = txtSearch;
                    List<CardNumbersExcel> datas = _cardnumbersServices.ExportExcel(obj);
                    var temp_head = helper.CreateTemplate("head");
                    var temp_body = helper.CreateTemplate("body");

                    helper.Insert(temp_head);
                    helper.InsertDatas(temp_body, datas);
                    helper.CopyWidth();
                }

                string fileName = "CardNumbers_";
                fileName += DateTime.Now.ToString("dd-MM-yyyy");

                FileStream fileStream = new FileStream(Server.MapPath(@"~/Download/Export/" + fileName + ".xlsx"), FileMode.Create, FileAccess.Write);
                st.WriteTo(fileStream);
                fileStream.Close();

                return Json(new { success = true, urlFile = "/Download/Export/" + fileName + ".xlsx", fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }



        }
        public JsonResult GetDataExist()
        {
            //WarehouseId = WarehouseId ?? User.WarehouseId;
            //if (WarehouseId == null)
            //{
            //    return Json(new { success = false, message = "Không có showroom để lấy thẻ!" }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            var listCardnumber = _cardnumbersServices.GetDataExist();
            return Json(new { success = false, data = listCardnumber }, JsonRequestBehavior.AllowGet);
            //}
        }
        public JsonResult CheckCardnumber(string Cardnumber)
        {
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Sync()
        {
            ResultMessageModel result = _cardnumbersServices.Sync();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteCardNumber(Select2Param obj)
        {
            int total = 0;
            var data = _cardnumbersServices.AutoCompleteCardNumber(obj, ref total);
            if (data == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, results = data, total = total }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CardNumber_AutoComplete(Select2Param obj)
        {
          
            var data = _cardnumbersServices.CardNumber_AutoComplete(obj);
           
                return Json(new { success = true, results = data, total = data.Count() }, JsonRequestBehavior.AllowGet);
           
        }
        public ActionResult Create()
        {
            return PartialView("_Create_Edit", new CardNumbersEntity());
        }
        public JsonResult Insert_Update(string CardNumberId)
        {
            try
            {
                if (_cardnumbersServices.CheckExistCode(CardNumberId))
                {
                    return Json(new { status = false, message = "Mã thẻ đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                }
                string message = "";
                var status = _cardnumbersServices.InsertCardNumber(CardNumberId, ref message);
                return Json(new { status , message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }

        }
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
                    bool CheckTemplate = true;
                    List<CardNumbersEntity> list = ExcelExtensionCustom.ReadtoList<CardNumbersEntity>(pathSave + fileImportExcel.FileName, ref listError, ref CheckTemplate)
                    .Select(t => { t.ImportSuccess = ""; return t; }).ToList();
                    if(!CheckTemplate || list == null)
                    {
                        return Json(new { success = false, message = "Template lỗi!" }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in list)
                    {
                        item.ImportSuccess = "";
                        if (_cardnumbersServices.CheckExistCode(item.CardNumberId))
                        {
                            item.ImportSuccess += "Mã thẻ đã tồn tại";
                        }
                    }
                    return Json(new { success = true, lstCard = list, listError }, JsonRequestBehavior.AllowGet);
                    //return UpdateImportExcel(list, listError);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = "Template lỗi!" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[UserAuthorize(Modules = new ActionModule[] { ActionModule.Category }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult UpdateImportExcel(List<CardNumbersEntity> data)
        {
            if (data.Count > 0)
            {
                data.ForEach(t =>
                {
                    if (t.CardNumberId == "")
                    {
                        t.ImportSuccess = "Không để trống mã thẻ";
                    }
                    else
                    {
                        if (!_cardnumbersServices.CheckExistCode(t.CardNumberId))
                        {
                            string message = "";
                            var cate = _cardnumbersServices.InsertCardNumber(t.CardNumberId,ref message);
                            if (cate == true) t.ImportSuccess = "1";
                            else t.ImportSuccess = message;
                        }
                        else
                        {
                            t.ImportSuccess = "Đã tồn tại";
                        }
                    }

                });
            }
            return Json(new { success = true, lstCard = data }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CheckRegister(string CardNumberId)
        //{
        //    bool CheckExist = _cardnumbersServices.CheckExistCode(CardNumberId);
        //    if (CheckExist)
        //    {
        //        bool CheckRegister = _cardnumbersServices.CheckRegister(CardNumberId);
        //        if (CheckRegister)
        //        {
        //            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Thẻ không thể dùng!" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        bool CheckInsert = _cardnumbersServices.CheckToInsert(CardNumberId);
        //        if (CheckInsert)
        //        {
        //            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //        }
        //        else return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //}
    }

}