using System.Web.Mvc;
using LIB;
using Framework.EF;
using System.Configuration;
using WebApplication.Authorize;
using LIB.CardNumbers;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using LibCore.Helpers;
using System;
using System.Web;
using OfficeOpenXml;
using static LIB.ExcelExtension;
using System.Net.Mail;
using LIB.Warehouses;

namespace WebApplication.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomer _customerServices;
        private readonly ICardNumbers _cardnumbersServices;
        private readonly IWarehouses _warehouseService;
        public CustomerController()
        {
            _customerServices = SingletonIpl.GetInstance<IplCustomer>();
            _cardnumbersServices = SingletonIpl.GetInstance<IplCardNumbers>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Add, ActionType.View, ActionType.Record })]
        public ActionResult Create()
        {
            //if (User.IsSupperAdmin)
            //{
            //    ViewBag.AllWarehouse = _warehouseService.GetAllData();
            //}
            return PartialView("_Create_Edit", new CustomerEntity());
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Edit, ActionType.View, ActionType.Record })]
        [HttpGet]
        public ActionResult Edit(long Id)
        {
            //if (User.IsSupperAdmin)
            //{
            //    ViewBag.AllWarehouse = _warehouseService.GetAllData();
            //}
            CustomerEntity cus = _customerServices.GetByID(Id);
            return PartialView("_Create_Edit", cus);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Edit, ActionType.View, ActionType.Record })]
        public JsonResult Insert_Update(CustomerEntity data)
        {
            object message = "";
            if (_customerServices.CustomerCheckExist(data, ref message))
            {
                //Đã trùng
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
            else if (data.CardNumberId != null) //update
            {
                if (_cardnumbersServices.CheckToInsert(data.CardNumberId, data.Id, ref message))
                {
                    CustomerEntity customer = _customerServices.Insert_Update(data, ref message);
                    if (customer == null)
                    {
                        return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { customer, success = true, message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
            else //insert
            {
                CustomerEntity customer = _customerServices.Insert_Update(data, ref message);
                if (customer == null)
                {
                    return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { customer, success = true, message }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]
        public ActionResult Detail(int CustomerId)
        {
            CustomerEntity customer = _customerServices.GetByID(CustomerId);
            return View(customer);
        }
        public ActionResult CardNumber()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AutoCompletedCustomer(Select2Param obj)
        {
            int total = 0;
            var data = _customerServices.AutoCompleteCustomer(obj, ref total);
            if (data == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, results = data, total = total }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult GetDataCustomer(bootstrapTableParam obj)
        {
            int total = 0;
            List<CustomerEntity> data = _customerServices.GetDataCustomer(obj, ref total);
            if (User.IsSupperAdmin)
            {
                data = data.Select(t => { t.AllowDelete = true; return t; }).ToList();
            }
            return Json(new { success = true, datas = data, total }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult GetCardNumber(bootstrapTableParam obj)
        {
            int total = 0;
            List<CustomerEntity> data = _customerServices.GetDataCustomer(obj, ref total);
            if (User.IsSupperAdmin)
            {
                data = data.Select(t => { t.AllowDelete = true; return t; }).ToList();
            }
            return Json(new { success = true, datas = data, total }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Edit, ActionType.View, ActionType.Record })]
        public ActionResult RenderCreateCustomer()
        {
            ViewBag.CardNumber = _cardnumbersServices.GetCardNumber();
            return PartialView("_CreateCustomer");
        }
        public JsonResult GetNewCardNumber()
        {
            string cardNumber = _cardnumbersServices.GetCardNumber();
            if (cardNumber == "")
            {
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, cardNumber }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult GetNewCodeAndCardNumber()
        {
            bool kq = true;
            string CustomerCode = _customerServices.GetNewCode();
            string CardNumber = _cardnumbersServices.GetCardNumber();
            //if (CardNumber == null || CardNumber == "") kq = false;//hết thẻ
            return Json(new { success = kq, CustomerCode, CardNumber }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult GetInfoCustomer(long? CustomerId)
        {
            if (CustomerId == null)
            {
                return PartialView("../Customer/_InfoCustomer", null);
            }
            else
            {
                CustomerEntity cus = _customerServices.GetByID(CustomerId.Value);
                return PartialView("../Customer/_InfoCustomer", cus);
            }
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult InfoCustomer(CustomerEntity customer)
        {
            return PartialView("../Customer/_InfoCustomer", customer ?? new CustomerEntity());
        }
        [HttpPost]
        public JsonResult GetCustomerByID(long CustomerId)
        {
            CustomerEntity cus = new CustomerEntity();
            if (CustomerId > 0)
            {
                cus = _customerServices.GetByID(CustomerId);
            }
            if (cus != null && cus.Point != 0)
            {
                cus.MoneyAllPoint = cus.Point * decimal.Parse(ConfigurationManager.AppSettings["MoneyOnePoint"].ToString());
            }
            return Json(new { customer = cus }, JsonRequestBehavior.AllowGet);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Delete })]
        public JsonResult DeleteCustomer(long CustomerId)
        {
            return Json(new { success = _customerServices.DeleteCustomer(CustomerId) }, JsonRequestBehavior.AllowGet);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Export })]
        public JsonResult Export(string txtSearch)
        {
            MemoryStream st = new MemoryStream();
            string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(@"~/Templates/Export/CustomersExport.xlsx");
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
                List<CustomerEntity> datas = _customerServices.GetDataCustomer(obj, ref totalRecord);

                var temp_head = helper.CreateTemplate("head");
                var temp_body = helper.CreateTemplate("body");

                helper.Insert(temp_head);
                helper.InsertDatas(temp_body, datas);
                helper.CopyWidth();
            }

            string fileName = "Customers_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy");

            FileStream fileStream = new FileStream(Server.MapPath(@"~/Download/Export/" + fileName + ".xlsx"), FileMode.Create, FileAccess.Write);
            st.WriteTo(fileStream);
            fileStream.Close();

            return Json(new { urlFile = "/Download/Export/" + fileName + ".xlsx", fileName }, JsonRequestBehavior.AllowGet);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Import })]
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
                    List<CustomerEntity> list = ReadtoList<CustomerEntity>(pathSave + fileImportExcel.FileName, ref listError).Select(t => { t.ImportMessage = ""; return t; }).ToList();
                    list.ForEach(t =>
                    {
                        if (t.CardNumberId != null)
                        {
                            //check cardnumberId
                            if (!_cardnumbersServices.CheckToUse(t.CardNumberId))
                            {
                                t.ImportMessage += "*Mã thẻ đã được sử dụng hoặc chưa kích hoạt.";
                            }
                        }
                        if (t.Name == null || t.Name == "")
                        {
                            t.ImportMessage += "*Tên khách hàng không được bỏ trống.";
                        }
                        if (t.Phone == null || t.Phone == "")
                        {
                            t.ImportMessage += "*Số điện thoại không được bỏ trống.";
                        }
                        else
                        {
                            try
                            {
                                int.Parse(t.Phone);
                                if (10 > t.Phone.Length || t.Phone.Length > 11)
                                {
                                    t.Phone = null;
                                    t.ImportMessage += "*Số điện thoại không hợp lệ!";
                                }
                            }
                            catch
                            {
                                t.Phone = null;
                                t.ImportMessage += "*Sai số điện thoại.";
                            }
                        }
                        //if (t.Email == null || t.Email == "")
                        //{
                        //    t.ImportMessage += "*Email không được bỏ trống.";
                        //}
                        //else
                        //{
                        //    try
                        //    {
                        //        if (t.Email != "")
                        //        {
                        //            MailAddress m = new MailAddress(t.Email);
                        //        }
                        //    }
                        //    catch
                        //    {
                        //        t.Email = null;
                        //        t.ImportMessage += "*Sai Email.";
                        //    }
                        //}
                    });
                    return Json(new { success = true, lstCus = list, listError }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Customer }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult UpdateImportExcel(List<CustomerEntity> data)
        {
            if (data.Count > 0)
            {
                data.ForEach(t =>
                {
                    object message = "";
                    if (_customerServices.CustomerCheckExist(t, ref message))
                    {
                        //Đã trùng
                        t.ImportMessage = message.ToString();
                    }
                    else if (t.CardNumberId != null && !_cardnumbersServices.CheckToUse(t.CardNumberId))
                    {
                        t.ImportMessage = "Mã thẻ đã được sử dụng hoặc không tồn tại";
                    }
                    else
                    {
                        CustomerEntity customer = _customerServices.Insert_Update(t, ref message);
                        if (customer == null)
                        {
                            t.ImportMessage = message.ToString();
                        }
                        else
                        {
                            t.ImportMessage = "1";
                        }
                    }
                });
            }
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllData(int? WarehouseId)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}