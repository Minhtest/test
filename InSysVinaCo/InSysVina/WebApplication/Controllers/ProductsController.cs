using Framework.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using LIB;
using WebApplication.Authorize;
using LIB.Product;
using LibCore.Helpers;
using LIB.Warehouses;
using System.Linq;
using System.Web;
using System.Configuration;
using static LIB.ExcelExtension;
using LIB.API;
using System.Threading.Tasks;
using LIB.ProductCate;
using WebApplication.Helpers;
using LIB.SyncSetting;

namespace WebApplication.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProduct _productService;
        private readonly ICategory _categoryService;
        private readonly IProductCate _productcateSevice;
        private readonly IWarehouses _warehouseService;
        private readonly IApiConnect _apiConnectService;
        private readonly IAPI _apiService;
        public ProductsController()
        {
            _productService = SingletonIpl.GetInstance<IplProduct>();
            _warehouseService = SingletonIpl.GetInstance<IplWarehouses>();
            _apiService = SingletonIpl.GetInstance<IplAPI>();
            _categoryService = SingletonIpl.GetInstance<IplCategory>();
            _productcateSevice = SingletonIpl.GetInstance<IplProductCate>();
            _apiConnectService = SingletonIpl.GetInstance<IplApiConnect>();
        }
        // GET: Products
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.View })]
        public ActionResult Index()
        {
            ViewBag.AllWarehouse = _warehouseService.GetAllData();
            var warehouseid = User.WarehouseId;
            if (warehouseid != null)
            {
                var res = _warehouseService.GetById(warehouseid.Value);
                ViewBag.IsSync = res.IsSync;
            }
            return View("Index");
        }
      
        public ActionResult Create()
        {
            var ApiWarehouseName = "Warehouse";// ConfigurationSettings.AppSettings["ApiWarehouseName"].ToString();
            var apiConfig = _apiConnectService.GetSelect2Data().Where(t => t.Name == ApiWarehouseName).FirstOrDefault();
            var listWarehouse = _warehouseService.GetAllSync(apiConfig.Id);
            List<CategoryEntity> list = _categoryService.GetAllWithLevel("");
            var data = Tree.getChildList(list, null);

            List<SelectListItem> lst = data.Select(t => new SelectListItem()
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
            ViewBag.CategoryList = lst;
            ViewBag.AllWarehouse = listWarehouse;
            return View("CreateOrEdit", new ProductEntity());
        }
        public ActionResult Edit(int? Id)
        {
            List<CategoryEntity> list = _categoryService.GetAllWithLevel("");
            var data = Tree.getChildList(list, null);
            var function = _productcateSevice.GetCateProuctId(Id.Value);
            List<SelectListItem> lst = data.Select(t => new SelectListItem()
            {
                Selected = function != null ? function.Where(f => f.CateId == t.Id).Count() > 0 ? true : false : false,
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
            ViewBag.CategoryList = lst;
            ViewBag.AllWarehouse = _warehouseService.GetAllData();
            var Product = _productService.GetByID(Id.Value);
            return View("CreateOrEdit", Product);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.View })]
        [HttpPost]
        public JsonResult InsertProduct(ProductEntity data, int? WarehouseId, int[] selectCate)
        {
            WarehouseId = WarehouseId ?? User.WarehouseId.Value;
            if (WarehouseId == null)
            {
                return Json(new { success = false, message = "Không có showroom" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (data.Id <= 0 || data.Id == null) // insert sản phẩm mới
                {
                    if (_productService.CheckByBarcode(data.Barcode, WarehouseId ?? User.WarehouseId.Value))
                    {
                        return Json(new { success = false, message = "Sản phẩm đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string message = "";
                        string message2 = "";
                        var ProductId = _productService.InsertOrUpdate(data, WarehouseId ?? User.WarehouseId.Value, ref message).Id;
                        var listProductCate = _productcateSevice.GetCateProuctId(ProductId.Value);

                        if (listProductCate.Count > 0)
                        {
                            foreach (var item in listProductCate)
                            {
                                _productcateSevice.Delete(item);
                            }
                        }
                        for (int i = 0; i < selectCate.Length; i++)
                        {
                            var model = new ProductCateEntity
                            {
                                ProductId = int.Parse(ProductId.ToString()),
                                CateId = selectCate[i]

                            };
                            var productcate = _productcateSevice.Insert(model, ref message2);

                        }
                        if (ProductId == null)
                        {
                            return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = true, message, type = (ProductId == null ? 0 : 1) }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else // Update product
                {
                    if (_productService.CheckByBarcode(data.Barcode, WarehouseId ?? User.WarehouseId.Value))
                    {
                        string message = "";
                        string message2 = "";
                        var ProductId = _productService.InsertOrUpdate(data, WarehouseId ?? User.WarehouseId.Value, ref message).Id;
                        var listProductCate = _productcateSevice.GetCateProuctId(ProductId.Value);

                        if (listProductCate.Count > 0)
                        {
                            foreach (var item in listProductCate)
                            {
                                _productcateSevice.Delete(item);
                            }
                        }
                        for (int i = 0; i < selectCate.Length; i++)
                        {
                            var model = new ProductCateEntity
                            {
                                ProductId = int.Parse(ProductId.ToString()),
                                CateId = selectCate[i]

                            };
                            var productcate = _productcateSevice.Insert(model, ref message2);

                        }
                        if (ProductId == null)
                        {
                            return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = true, message, type = (ProductId == null ? 0 : 1) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Sản phẩm này chưa tồn tại!" }, JsonRequestBehavior.AllowGet);
                    }
                }


            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.View })]
        [HttpPost]
        public JsonResult GetDataProducts(bootstrapTableParam obj, int? WarehouseId)
        {
            try
            {
                int totalRecord = 0;
                List<ProductEntity> datas = _productService.GetDataProduct(obj, User.WarehouseId ?? WarehouseId, ref totalRecord);
                datas.All(t => { t.AllowEdit = true; return true; });
                if (User.IsStaff)
                {
                    datas.All(t => { t.AllowEdit = false; return true; });
                }
                return Json(new { success = true, data = datas, total = totalRecord, AllowEdit = (User.IsStaff == true ? false : true) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return JsonResultError(ex);
            }
        }
        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Sync })]
        public JsonResult ProductSync(int? WarehouseId)
        {
            try
            {
                WarehouseId = WarehouseId ?? User.WarehouseId;
                if (WarehouseId == null)
                {
                    return Json(new ResultMessageModel("Không có Showroom!", 3), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ResultMessageModel kq = _apiService.API_Warehouses_SyncNewProduct((int)WarehouseId, _apiConnectService, _warehouseService, _productService).Result;
                    if (kq.type == 1)
                    {
                        //tạo cookie productSync
                        if (Request.Cookies["productSync"] == null)
                        {
                            HttpCookie productSyncCookie = new HttpCookie("productSync");
                            productSyncCookie.Value = "1";
                            productSyncCookie.Expires = DateTime.Today.AddDays(1);
                            Response.Cookies.Add(productSyncCookie);
                        }
                        Log.Info("ProductSyncNew Success");
                    }
                    return Json(kq, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AutoCompletedProduct(Select2Param param, int? WarehouseId)
        {
            try
            {
                int totalRecord = 0;
                bootstrapTableParam obj = new bootstrapTableParam();
                obj.limit = 10;
                obj.offset = (param.page - 1) * 10;
                obj.search = param.term;
                List<ProductEntity> data = _productService.GetDataProductHaveQuantity(obj, WarehouseId ?? User.WarehouseId.Value, ref totalRecord);
                return Json(new { success = true, results = data, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Report })]
        public ActionResult Report()
        {
            ViewBag.AllWarehouse = _warehouseService.GetAllData();
            return View("Report");
        }
        public ActionResult GetDataProductReport(bootstrapTableParam obj, DateTime? startDate, DateTime? endDate, int? WarehouseId)
        {
            try
            {
                int totalRecord = 0;
                var datas = _productService.GetProductReport(obj, startDate, endDate, User.WarehouseId ?? WarehouseId, ref totalRecord);
                return Json(new { success = true, data = datas, total = totalRecord }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ExportReport(string txtSearch, DateTime? startDate, DateTime? endDate, int? WarehouseId)
        {
            try
            {
                MemoryStream st = new MemoryStream();
                string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(ConfigurationManager.AppSettings["Template_Export_ProductsReport"]);
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
                    var datas = _productService.GetProductReport(obj, startDate, endDate, WarehouseId ?? User.WarehouseId, ref totalRecord);

                    var temp_head = helper.CreateTemplate("head");
                    var temp_body = helper.CreateTemplate("body");

                    helper.Insert(temp_head);
                    helper.InsertDatas(temp_body, datas);
                    helper.CopyWidth();
                }

                string fileName = "ProductsReport";
                if (startDate != null) fileName += "_" + startDate.Value.ToString("dd-MM-yyyy");
                if (endDate != null) fileName += "_" + endDate.Value.ToString("dd-MM-yyyy");
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

        [HttpPost]
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Record })]
        public JsonResult UpdateProduct(ProductEntity product, int? WarehouseId)
        {
            try
            {
                return Json(new { success = _productService.UpdateSellPrice(product.Id.Value, product.SellPriceShop.Value, WarehouseId ?? User.WarehouseId.Value) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Export })]
        public JsonResult Export(string txtSearch, int? WarehouseId)
        {
            MemoryStream st = new MemoryStream();
            string UrlTemplate = ControllerContext.HttpContext.Server.MapPath(@"~/Templates/Export/ProductsExport.xlsx");
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
                List<ProductEntity> datas = _productService.GetDataProduct(obj, WarehouseId ?? User.WarehouseId, ref totalRecord);

                var temp_head = helper.CreateTemplate("head");
                var temp_body = helper.CreateTemplate("body");

                helper.Insert(temp_head);
                helper.InsertDatas(temp_body, datas);
                helper.CopyWidth();
            }

            string fileName = "Products_";
            fileName += DateTime.Now.ToString("dd-MM-yyyy");

            FileStream fileStream = new FileStream(Server.MapPath(@"~/Download/Export/" + fileName + ".xlsx"), FileMode.Create, FileAccess.Write);
            st.WriteTo(fileStream);
            fileStream.Close();

            return Json(new { urlFile = "/Download/Export/" + fileName + ".xlsx", fileName }, JsonRequestBehavior.AllowGet);
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult ReadFileImportExcel(FormCollection formCollection)
        {
          
            try
            {
                HttpPostedFileBase fileImportExcel = Request.Files["fileImportExcel"];
                var IDWarehouse = int.Parse(Request["IDWarehouse"]);
                if (fileImportExcel == null)
                {
                    return Json(new { success = false, message = "Tập tin không có!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //save file
                    string pathSave = Server.MapPath(ConfigurationManager.AppSettings["PathUploadFileImport"]);
                    fileImportExcel.SaveAs(pathSave + fileImportExcel.FileName);
                    //read file to list model
                    List<string> listError = new List<string>();
                    List<ProductEntity> list = ReadtoList<ProductEntity>(pathSave + fileImportExcel.FileName, ref listError).Select(t => { t.ImportStatus = ""; return t; }).ToList();
                    List<int> CateId = new List<int>();
                    foreach (var t in list)
                    {
                        t.ImportStatus = "";
                        if (t.Barcode == null || t.Barcode == "")
                        {
                            t.ImportStatus += "*Barcode không trống.";
                            
                     
                        }
                        if (t.ProductName == null || t.ProductName == "")
                        {
                            t.ImportStatus += "*Tên sản phẩm không trống.";
                    
                        }
                        if (t.ProductCategory == null || t.ProductCategory == "")
                        {
                            t.ImportStatus += "*Danh mục không trống.";
                
                        }
                        else
                        {
                            
                            string[] CateCode = System.Text.RegularExpressions.Regex.Split(t.ProductCategory, ",");
                            foreach (var _catecode in CateCode)
                            {
                                var cate = _categoryService.GetByCode(_catecode.Trim());
                                if (cate != null)
                                {
                                    CateId.Add((int)cate.Id);
                                }
                                else
                                {
                                    t.ImportStatus += "*Mã danh mục " + _catecode.Trim() + " không hợp lệ";
          
                                }

                            }

                        }
                        if (t.InventoryNumber == 0)
                        {
                            t.ImportStatus += "*Số lượng > 0.";
      
                        }
                        if (t.ComputeUnit == null || t.ComputeUnit == "")
                        {
                            t.ImportStatus += "*Đơn vị tính không trống.";
                      
                        }
                        if (t.Price == null)
                        {
                            t.ImportStatus += "*Giá nhập không trống.";
                      
                        }
                        if (t.SellPrice == null)//coi như giá bán của shop
                        {
                            t.ImportStatus += "*Giá bán không trống.";
                        
                        }
                        //if (t.ExpiredDate == null)
                        //{
                        //    t.ImportStatus += "*Hạn sử dụng không trống.";
                         
                        //}
                        //if (t.ImportStatus == "")
                        //{
                        //    InsertProduct(t, IDWarehouse, CateId.ToArray());
                        //}
                        //else
                        //{
                        //    continue;
                        //}

                    }

                    return Json(new { success = true, list, listError }, JsonRequestBehavior.AllowGet);
                    //return UpdateImportProducts(list,IDWarehouse, listError);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { success = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [UserAuthorize(Modules = new ActionModule[] { ActionModule.Product }, ActionType = new ActionType[] { ActionType.Import })]
        public JsonResult UpdateImportProducts(List<ProductEntity> data, int? WarehouseId, List<string> listError)
        {
            if (data.Count > 0)
            {
                data.ForEach(p =>
                {
                    var productCheck = _productService.GetByBarcode(p.Barcode, WarehouseId ?? User.WarehouseId.Value);
                    if (productCheck == null)
                    {
                        List<int?> CateId = new List<int?>();
                        string status = "";
                        string[] CateCode = System.Text.RegularExpressions.Regex.Split(p.ProductCategory, ",");
                        foreach (var _catecode in CateCode)
                        {
                            var cate = _categoryService.GetByCode(_catecode.Trim());
                            if (cate != null)
                            { CateId.Add(cate.Id); }
                        }
                        //p.ProductCategory = null;
                        string message2 = "";
                        ProductEntity product = _productService.InsertOrUpdate(p, WarehouseId ?? User.WarehouseId.Value, ref status);
                        foreach (var _CateId in CateId)
                        {
                            var model = new ProductCateEntity
                            {
                                ProductId = int.Parse(product.Id.ToString()),
                                CateId = _CateId.Value

                            };
                            var productcate = _productcateSevice.Insert(model, ref message2);

                        }
                        if (product == null) p.ImportStatus = status;
                        else p.ImportStatus = "1";
                    }
                    else
                    {
                        string status = "";
                        List<int?> CateId = new List<int?>();
                        string[] CateCode = System.Text.RegularExpressions.Regex.Split(p.ProductCategory, ",");
                        foreach (var _catecode in CateCode)
                        {
                            var cate = _categoryService.GetByCode(_catecode.Trim());
                            if (cate != null)
                            { CateId.Add(cate.Id); }
                        }
                        //p.ProductCategory = null;
                        productCheck.InventoryNumber = productCheck.InventoryNumber + p.InventoryNumber;
                        ProductEntity product = _productService.InsertOrUpdate(productCheck, WarehouseId ?? User.WarehouseId.Value, ref status);
                        string message2 = "";
                        foreach (var _CateId in CateId)
                        {
                            var model = new ProductCateEntity
                            {
                                ProductId = int.Parse(product.Id.ToString()),
                                CateId = _CateId.Value

                            };
                            var productcate = _productcateSevice.Insert(model, ref message2);

                        }
                        if (status == "")
                        {
                            p.ImportStatus = "2";
                        }
                    }
                });
            }
            return Json(new { success = true, list = data, listError  }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChartTop10(int opSearch, int? IDWarehouse)
        {
            List<ProductsChartTop10Model> list = _productService.GetProducts_ChartTop10(opSearch, IDWarehouse ?? User.WarehouseId ?? 0);
            if (list != null)
            {
                List<string> listLabel = list.Select(t => t.Barcode).ToList();
                List<int> listQuantity = list.Select(t => t.Quantity).ToList();
                return Json(new { success = true, listLabel, listQuantity }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}