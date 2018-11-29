using Framework.EF;
using LIB;
using LIB.Orders;
using LIB.Report;
using LIB.Warehouses;
using LibCore.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Authorize;

namespace WebApplication.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        private readonly IOrder _orderService;
        private readonly IWarehouses _warehouseService;
        public ReportController()
        {
            _orderService = SingletonIpl.GetInstance<IplOrder>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
        }
        [Authorize.UserAuthorize(Modules = new ActionModule[] { ActionModule.Report }, ActionType = new ActionType[] { ActionType.View, ActionType.Export })]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize.UserAuthorize(Modules = new ActionModule[] { ActionModule.Report }, ActionType = new ActionType[] { ActionType.View, ActionType.Export })]
        public ActionResult DetailGeneral()
        {
            ViewBag.AllWarehouse = _warehouseService.GetAllData();
            return View("DetailGeneral");
        }
        public JsonResult GetDataDetailGeneral(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, string type, int? CreatedSearchId, int? WarehouseId, int? CustomerSearchId)
        {
            int totalRecord = 0;
            List<DetailGeneralModel> datas = _orderService.GetDataDetailGeneral(obj, opType, startDate, endDate, User.WarehouseId ?? WarehouseId, CreatedSearchId, CustomerSearchId, ref totalRecord);
            if (datas == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { success = true, data = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportDetailGeneral(string txtSearch, int opType, DateTime? startDate, DateTime? endDate, int? CreatedSearchId, int? CustomerSearchId, int? WarehouseId)
        {
            try
            {
                MemoryStream st = new MemoryStream();
                string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(ConfigurationManager.AppSettings["Template_Export_DetailGeneralReport"]);
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
                    List<DetailGeneralModel> datas = _orderService.GetDataDetailGeneral(obj, opType, startDate, endDate, User.WarehouseId ?? WarehouseId, CreatedSearchId, CustomerSearchId, ref totalRecord);

                    var temp_head = helper.CreateTemplate("head");
                    var temp_group = helper.CreateTemplate("group");
                    var temp_body = helper.CreateTemplate("body");
                    var temp_footer = helper.CreateTemplate("footer");

                    helper.Insert(temp_head);
                    if (datas.Count > 0)
                    {
                        var ordercode = "";
                        for (int i = 0; i < datas.Count; i++)
                        {
                            if (ordercode == "")
                            {
                                helper.InsertData(temp_group, new { group = datas[i].OrderCode + " - " + datas[i].OrderDate.ToString("dd/MM/yyyy HH:mm:ss") + " - " + datas[i].CreatedBy_UserName + " - " + datas[i].CustomerName });
                                helper.InsertData(temp_body, datas[i]);
                            }
                            else
                            {
                                if (datas[i].OrderCode == ordercode)
                                {
                                    helper.InsertData(temp_body, datas[i]);
                                }
                                else
                                {
                                    helper.InsertData(temp_footer, new
                                    {
                                        datas[i - 1].TotalPrice,
                                        datas[i - 1].Discount,
                                        datas[i - 1].PointUsed,
                                        datas[i - 1].GrandTotal,
                                        datas[i - 1].PaidGuests,
                                        datas[i - 1].RefundMoney
                                    });
                                    helper.InsertData(temp_group, new { group = datas[i].OrderCode + " - " + datas[i].OrderDate.ToString("dd/MM/yyyy HH:mm:ss") + " - " + datas[i].CreatedBy_UserName + " - " + datas[i].CustomerName });
                                    helper.InsertData(temp_body, datas[i]);
                                }

                            }
                            ordercode = datas[i].OrderCode;
                            if (i == (datas.Count - 1))
                            {
                                helper.InsertData(temp_footer, new
                                {
                                    datas[i].TotalPrice,
                                    datas[i].Discount,
                                    datas[i].PointUsed,
                                    datas[i].GrandTotal,
                                    datas[i].PaidGuests,
                                    datas[i].RefundMoney
                                });
                            }
                        }
                    }
                    helper.CopyWidth();
                }
                string fileName = "DetailGeneral";
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
        public JsonResult GetDataCharDetailGeneral(bootstrapTableParam obj, int opType, DateTime? startDate, DateTime? endDate, string type, int? CreatedSearchId, int? WarehouseId)
        {
            obj.limit = 0;
            obj.offset = 0;
            startDate = null;
            int totalRecord = 0;
            decimal totalPrice = 0;
            List<OrderReportModel> datas = _orderService.GetDataOrdersReport(obj, opType, startDate, endDate, User.WarehouseId ?? WarehouseId, null, CreatedSearchId, 0, 0, ref totalRecord, ref totalPrice);
            if (datas == null)
            {
                return Json(new { success = false, message = "Lấy dữ liệu thất bại!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> lstLabels = new List<string>();
                List<int> lstQuantitys = new List<int>();
                List<decimal> lstSales = new List<decimal>();
                if (opType == 0)
                {
                    //Ngày
                    int hourNow = DateTime.Now.Hour;
                    for (int i = 0; i <= hourNow; i++)
                    {
                        lstLabels.Add(i + ":00");
                        lstQuantitys.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Hour == i).Count());
                        lstSales.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Hour == i).Sum(t => t.GrandTotal));
                    }
                }
                else if (opType == 1)
                {
                    //Tháng
                    int dayNow = DateTime.Now.Day;
                    int monthNow = DateTime.Now.Month;
                    for (int i = 1; i <= dayNow; i++)
                    {
                        lstLabels.Add(i + "/" + monthNow);
                        lstQuantitys.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Day == i).Count());
                        lstSales.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Day == i).Sum(t => t.GrandTotal));
                    }
                }
                else if (opType == 2)
                {
                    //Năm
                    int monthNow = DateTime.Now.Month;
                    int yearNow = DateTime.Now.Year;
                    for (int i = 1; i <= monthNow; i++)
                    {
                        lstLabels.Add(i + "/" + yearNow);
                        lstQuantitys.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Month == i).Count());
                        lstSales.Add(datas.Where(t => ((DateTime)t.OrderDateTime).Month == i).Sum(t => t.GrandTotal));
                    }
                }
                return Json(new { lstLabels, lstQuantitys, lstSales }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}