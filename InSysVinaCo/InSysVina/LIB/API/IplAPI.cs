using Dapper;
using LIB.Product;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using LIB.Tokens;
using LIB.CardNumbers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LIB.SyncSetting;
using LIB.Warehouses;
using LIB.Orders;
using System.Xml;

namespace LIB.API
{
    public class IplAPI : BaseService<ProductEntity, long>, IAPI
    {
        #region API Warehouses
        public string GetBearerTokenToWarehouse(string apiToken)
        {
            try
            {
                DynamicParameters paramClient = new DynamicParameters();
                ClientEntity clientEntity = unitOfWork.Procedure<ClientEntity>("sp_Clients_ConnectWarehouses", paramClient).SingleOrDefault();
                if (clientEntity == null) return null;
                else
                {
                    var client_secret = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientEntity.SecretValue));
                    var strData = new Dictionary<string, string>
                    {
                        {"username", clientEntity.UserName},
                        {"password",clientEntity.PassWord},
                        {"client_id",clientEntity.SecretName },
                        {"client_secret", client_secret},
                        {"grant_type",clientEntity.Type }
                    };

                    var result = APIExtension.PostKeyValue(strData, apiToken);
                    if (result.IsSuccessStatusCode)
                    {
                        string jsonToken = result.Content.ReadAsStringAsync().Result;
                        TokenModel token = new JavaScriptSerializer().Deserialize<TokenModel>(jsonToken);
                        ////end => Get TOKEN
                        return token.token_type + " " + token.access_token;
                    }
                    else
                    {
                        Log.Error(result);
                        return null;
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public async Task<ResultMessageModel> API_Warehouses_SyncNewProduct(int WarehouseId, IApiConnect apiConnectService, IWarehouses warehousesService, IProduct productService)
        {
            try
            {
                
                var tokenApi = ConfigurationSettings.AppSettings["tokenApi"].ToString();
                var syncProductApi = ConfigurationSettings.AppSettings["syncProductApi"].ToString();
                int PageSize = int.Parse(ConfigurationSettings.AppSettings["pageSizeProduct"].ToString());
                var ApiWarehouseName = "Warehouse"; //ConfigurationSettings.AppSettings["ApiWarehouseName"].ToString();
                var apiConfig = apiConnectService.GetSelect2Data().Where(t => t.Name == ApiWarehouseName).FirstOrDefault();

               

                if (apiConfig != null)
                {
                    var listWarehouse = warehousesService.GetAllSync(apiConfig.Id);
                    if (listWarehouse != null && listWarehouse.FirstOrDefault(t=>t.Id == WarehouseId) != null)
                    {
                        string token = GetBearerTokenToWarehouse(apiConfig.Api + tokenApi);
                        Log.Info("Mã token:" + token);
                        if (token != null && token != "")
                        {
                            HttpClient client = new HttpClient();

                            //var WarehouseIds = string.Join(",", listWarehouse.Select(w => w.Id.ToString()).ToArray());
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("Authorization", token);
                            int PageIndex = 1;
                            int TotalRow = 100;
                            while ((PageIndex - 1) * PageSize <= TotalRow)
                            {
                                var data = JsonConvert.SerializeObject(new ProductFilter
                                {
                                    WarehouseIds = listWarehouse.FirstOrDefault(t => t.Id == WarehouseId).Id.ToString(),
                                    PageIndex = PageIndex,
                                    PageSize = PageSize
                                });
                                Log.Info("Data after serilze WarehousesId:" + data.ToString());
                                var result = client.PostAsync(apiConfig.Api + syncProductApi, new StringContent(data, Encoding.UTF8, "application/json")).Result;


                                if (result.IsSuccessStatusCode)
                                {
                                    string jsonProductSync = result.Content.ReadAsStringAsync().Result;

                                    var listProduct = JsonConvert.DeserializeObject<ProductResult>(jsonProductSync);

                                    if (listProduct.Status)
                                    {
                                        TotalRow = listProduct.TotalRows;
                                        Log.Info( "Data receive between "+ (PageIndex - 1) * PageSize + " -> " +((PageIndex - 1) * PageSize + PageSize) + " | Total Row: " + TotalRow);
                                        if (TotalRow == 0)
                                            break;

                                        var xmlData = XMLHelper.SerializeXML<List<LIB.Product.Product>>(listProduct.Data);

                                        
                                        Log.Info("Data receive from post api:" + jsonProductSync);
                                        productService.SaveSyncData(xmlData);

                                    }
                                    Log.Info("Get Product success");
                                   
                                }
                                else
                                {
                                    Log.Error("Get Product fail");
                                }
                                PageIndex++;
                            }

                            Log.Info("End while!");
                            return new ResultMessageModel("Đồng bộ thành công!", 1);
                        }

                        else
                        {
                            Log.Error("Get token fail!");
                            return new ResultMessageModel("Không lấy được token!", 3);
                        }

                    }
                    else
                    {
                        Log.Error("Warehouse not found");
                        return new ResultMessageModel("Kho chưa bật đồng bộ!", 3);
                    }
                }
                else
                {
                    Log.Error("Api not found");
                    return new ResultMessageModel("Không lấy đc API theo tên  \""+ ApiWarehouseName + "\"!", 3);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ResultMessageModel("Lỗi truyền dữ liệu!", 3);
            }
        }
        public async Task<ResultMessageModel> API_Warehouses_SyncDataProduct(int WarehouseId, IApiConnect apiConnectService, IWarehouses warehousesService, IOrder orderService)
        {
            try
            {
                Log.Info("===============Start Sync Orders! ====================");
                var tokenApi = ConfigurationSettings.AppSettings["tokenApi"].ToString();
                var postOrderApi = ConfigurationSettings.AppSettings["postOrderApi"].ToString();
                int PageSize = 1;
                var ApiWarehouseName = "Warehouse";// ConfigurationSettings.AppSettings["ApiWarehouseName"].ToString();
                var apiConfig = apiConnectService.GetSelect2Data().Where(t => t.Name == ApiWarehouseName).FirstOrDefault();
                int TotalRows = 0;
                Log.Info("API:" + apiConfig.Api + tokenApi);
                
                if (apiConfig != null)
                {
                    var listWarehouse = warehousesService.GetAllSync(apiConfig.Id);
                    if (listWarehouse != null && (listWarehouse.FirstOrDefault(t=>t.Id == WarehouseId) != null || WarehouseId == 0))
                    {
                        string token = GetBearerTokenToWarehouse(apiConfig.Api + tokenApi);
                        Log.Info("Mã token:" + token);
                        if (token != null && token != "")
                        {
                            HttpClient client = new HttpClient();
                   
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("Authorization", token);
                            int PageIndex = 1;
                            while ((PageIndex - 1) * PageSize <= TotalRows)
                            {
                                string xml = orderService.GetOrderSubmit(1, PageSize, apiConfig.Id, WarehouseId == 0? 0: listWarehouse.FirstOrDefault(t => t.Id == WarehouseId).Id, ref TotalRows);
                                if (TotalRows == 0)
                                {
                                    Log.Info("Not have Orders to post:" + xml);
                                    break;
                                }
                                Log.Info("Get order submit:" + xml);

                                var data = XMLHelper.DeserializeXML<OrderPost>(xml);

                                //convert xml -> json
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(xml);
                                var jsonData = JsonConvert.SerializeObject(data);


                                Log.Info("Data after convert xml -> json:" + jsonData);
                                Log.Info("API Order: "+ apiConfig.Api + postOrderApi);

                                var result = client.PostAsync(apiConfig.Api + postOrderApi, new StringContent(jsonData, Encoding.UTF8, "application/json")).Result;
                                Log.Info("Result" + result.RequestMessage);

                                if (result.IsSuccessStatusCode)
                                {
                                    var jsonProductSync = result.Content.ReadAsStringAsync().Result;

                                    var listOrder = JsonConvert.DeserializeObject<OrderResponse>(jsonProductSync);

                                    if (listOrder.Status)
                                    {
                                        string xmlData = XMLHelper.SerializeXML<List<OrderResult>>(listOrder.OrderResults);
                                        
                                        orderService.SaveOrderResponse(xmlData);
                                        Log.Info("Saved order response: " + xmlData);
                                    }
                                    else
                                    {
                                        Log.Info("Don't know Status: " + listOrder.Status);
                                    }

                                }
                                else
                                {
                                    Log.Error("Get order fail");
                                }
                                PageIndex++;
                            }

                            Log.Info("End while!");
                            return new ResultMessageModel("Đồng bộ thành công!", 1);
                        }

                        else
                        {
                            Log.Error("Get token fail!");
                            return new ResultMessageModel("Không lấy được token!", 3);
                        }

                    }
                    else
                    {
                        Log.Error("Warehouse not found");
                        return new ResultMessageModel("Kho chưa bật đồng bộ!", 3);
                    }
                }
                else
                {
                    Log.Error("Api not found");
                    return new ResultMessageModel("Không lấy đc API theo tên  \"" + ApiWarehouseName + "\"!", 3);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ResultMessageModel("Lỗi truyền dữ liệu!", 3);
            }
        }
        public bool API_Warehouses_SyncWarehouse(IApiConnect apiConnectService)
        {
            try
            {
                var tokenApi = ConfigurationSettings.AppSettings["tokenApi"].ToString();
                var getWarehouseApi = ConfigurationSettings.AppSettings["URL_Warehouses_SyncWarehouse"].ToString();
                var ApiWarehouseName = "Warehouse";// ConfigurationSettings.AppSettings["ApiWarehouseName"].ToString();
                var apiConfig = apiConnectService.GetSelect2Data().Where(t => t.Name == ApiWarehouseName).FirstOrDefault();
                
                string TokenToWarehouse = GetBearerTokenToWarehouse(apiConfig.Api + tokenApi);
                Log.Info("Token" + TokenToWarehouse);
                if (TokenToWarehouse == null) return false;
                else
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToWarehouse);

                    Log.Info("API: " + apiConfig.Api + getWarehouseApi);
                    var result2 = client.PostAsync(apiConfig.Api + getWarehouseApi, null).Result;

                    Log.Info("" + result2.IsSuccessStatusCode);

                    if (result2.IsSuccessStatusCode)
                    {
                        string jsonResult = result2.Content.ReadAsStringAsync().Result;
                        client.Dispose();
                        //check code
                        bool code = (bool)JObject.Parse(jsonResult)["code"];
                        if (code)
                        {
                            List<WarehouseSyncModel> listWarehouse = new JavaScriptSerializer().Deserialize<List<WarehouseSyncModel>>(JObject.Parse(jsonResult)["data"].ToString());
                            string xml = XMLHelper.SerializeXML<List<WarehouseSyncModel>>(listWarehouse).Replace("xsi:nil=\"true\"", "");
                            DynamicParameters paramUpdate = new DynamicParameters();
                            paramUpdate.Add("@xml", xml);
                            return unitOfWork.ProcedureExecute("API_Warehouses_Sync", paramUpdate);
                        }
                        else
                        {
                            Log.Error(JObject.Parse(jsonResult)["message"].ToString());
                            return false;
                        }
                    }
                    else
                    {
                        Log.Error(result2);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        #endregion
        #region API Momart
        public string GetBearerTokenToMomart()
        {
            try
            {
                DynamicParameters paramClient = new DynamicParameters();
                ClientEntity clientEntity = unitOfWork.Procedure<ClientEntity>("sp_Clients_ConnectMomart", paramClient).SingleOrDefault();
                Log.Info(clientEntity);
                if (clientEntity == null) return null;
                else
                {
                    var dicData = new Dictionary<string, string>
                    {
                        {"userName",clientEntity.UserName },
                        {"password",clientEntity.PassWord},
                        {"grant_type",clientEntity.Type }
                    };
                    var result = APIExtension.PostKeyValue(dicData, ConfigurationManager.AppSettings["API_Momart_GetTokenMomart"]);
                    Log.Info("result:" + result);
                    if (result.IsSuccessStatusCode)
                    {
                        string jsonToken = result.Content.ReadAsStringAsync().Result;
                        TokenModel token = new JavaScriptSerializer().Deserialize<TokenModel>(jsonToken);
                        ////end => Get TOKEN
                        return token.token_type + " " + token.access_token;
                    }
                    else
                    {
                        Log.Error(result);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public int API_Momart_GetDiscount(string couponCode, decimal TotalPrice, ref DiscountResult discount, ref DiscountModel discountObj)
        {
            try
            {
                string TokenToMomart = GetBearerTokenToMomart();
                if (TokenToMomart == null) return -1;
                else
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToMomart);
                    var resultDiscount = client.GetAsync(ConfigurationManager.AppSettings["API_Momart_GetDiscount"] + couponCode).Result;
                    string jsonDiscount = resultDiscount.Content.ReadAsStringAsync().Result;
                    if (couponCode.Trim().Length == 16)
                    {
                        API_DiscountCardCallModel discountCardCallModel = new JavaScriptSerializer().Deserialize<API_DiscountCardCallModel>(jsonDiscount);
                        if (discountCardCallModel.Code == 0)
                        {
                            return 0;
                        }
                        else
                        {
                            if (discountCardCallModel.Data.IsActive == false)
                            {
                                return 1;//hết hạn
                            }
                            else
                            {
                                if (DateTime.Now < discountCardCallModel.Data.ActiveDate)
                                {
                                    return 2;//chưa áp dụng
                                }
                                else
                                {
                                    discountObj = new DiscountModel()
                                    {
                                        Percentage = discountCardCallModel.Data.PercentDiscount,
                                        Type = 2,
                                        Code = couponCode
                                    };

                                    //2: Giảm theo phần trăm
                                    decimal DiscountPrice = TotalPrice * discountCardCallModel.Data.PercentDiscount / 100;
                                    discount.TotalPriceNew = TotalPrice - DiscountPrice;
                                    discount.DiscountValue = discountCardCallModel.Data.PercentDiscount;
                                    discount.type = 2;
                                    return 4;
                                }
                            }
                        }
                    }
                    else
                    {
                        API_DiscountCallModel discountCallModel = new JavaScriptSerializer().Deserialize<API_DiscountCallModel>(jsonDiscount);

                        if (discountCallModel.Code == 0)
                        {
                            return 0;
                        }
                        else
                        {
                            if (discountCallModel.Data.Status == false || DateTime.Now > discountCallModel.Data.EndDate || discountCallModel.Data.LimitationTimes <= 0)
                            {
                                return 1;//hết hạn
                            }
                            else
                            {
                                if (DateTime.Now < discountCallModel.Data.StartDate)
                                {
                                    return 2;//chưa áp dụng
                                }
                                else
                                {
                                    discountObj = discountCallModel.Data;
                                    if (TotalPrice < discountCallModel.Data.OrderValueCondition)
                                    {
                                        return 3;//Chưa đủ điều kiện
                                    }
                                    else
                                    {
                                        //Đủ điều kiện
                                        discount.type = discountCallModel.Data.Type.Value;
                                        if (discountCallModel.Data.Type == 1)
                                        {
                                            //Giảm theo giá trị
                                            discount.TotalPriceNew = TotalPrice - discountCallModel.Data.Amount.Value;

                                            discount.DiscountValue = discountCallModel.Data.Amount.Value;
                                        }
                                        else
                                        {
                                            //2: Giảm theo phần trăm
                                            decimal DiscountPrice = TotalPrice * discountCallModel.Data.Percentage.Value / 100;
                                            if (DiscountPrice > discountCallModel.Data.MaxValue) DiscountPrice = discountCallModel.Data.MaxValue.Value;
                                            discount.TotalPriceNew = TotalPrice - DiscountPrice;
                                            discount.DiscountValue = discountCallModel.Data.Percentage.Value;
                                        }
                                        return 4;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1;
            }
        }
        public ResultMessageModel API_Momart_GetCardNumbers()
        {
            try
            {
                string TokenToMomart = GetBearerTokenToMomart();
                if (TokenToMomart == null) return null;
                else
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToMomart);
                    var resultCardnumbers = client.GetAsync(ConfigurationManager.AppSettings["API_Momart_GetCardNumbers"]).Result;
                    string jsonCardnumbers = resultCardnumbers.Content.ReadAsStringAsync().Result;

                    if ((int)JObject.Parse(jsonCardnumbers)["Code"] == 0)
                    {
                        return new ResultMessageModel((string)JObject.Parse(jsonCardnumbers)["Message"], 2);
                    }
                    else
                    {
                        var jsonObj = JObject.Parse(jsonCardnumbers)["Data"].ToList();

                        List<CardNumbersEntity> list = new List<CardNumbersEntity>();
                        jsonObj.ForEach(t =>
                        {
                            list.Add(new CardNumbersEntity()
                            {
                                CardNumberId = t.ToString()
                            });
                        });
                        //insert to db

                        if (list != null && list.Count > 0)
                        {
                            string strXml = XMLHelper.SerializeXML<List<CardNumbersEntity>>(list).Replace("xsi:nil=\"true\"", "");
                            Log.Info(strXml);
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@strXml", strXml);
                            bool kq = unitOfWork.ProcedureExecute("sp_CardNumbers_Insert", param);
                            if (kq)
                            {
                                return new ResultMessageModel("Đồng bộ thành công!", 1);
                            }
                            else
                            {
                                return new ResultMessageModel("Đồng bộ thất bại!", 3);
                            }
                        }
                        else
                        {
                            return new ResultMessageModel("Không có thẻ để đồng bộ!", 2);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ResultMessageModel("Đồng bộ thất bại!", 3);
            }

        }
        public bool API_Momart_CheckCardNumber(CustomerEntity customer, ref object message)
        {
            try
            {
                //customer.CardNumberId = customer.CardNumber;
                string TokenToMomart = GetBearerTokenToMomart();
                if (TokenToMomart == null) return false;
                else
                {

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToMomart);
                    customer.CreateDate = DateTime.Now;
                    var content2 = new StringContent(new JavaScriptSerializer().Serialize(customer), Encoding.UTF8, "application/json");
                    var resultCardnumbers = client.PostAsync(APIExtension.GetValueAppSettings("API_Momart_CheckCustomerCardNumber"), content2).Result;
                    string jsonCardnumbers = resultCardnumbers.Content.ReadAsStringAsync().Result;
                    int check = int.Parse(JObject.Parse(jsonCardnumbers)["Code"].ToString());
                    //1: dùng được  2: thẻ đã tồn tại, 3:khách hàng đã tồn tại, 4: mã thẻ không tồn tại
                    if (check == 1)
                    {
                        return true;
                    }
                    else
                    {
                        message = JObject.Parse(jsonCardnumbers)["Message"].ToString();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public bool API_Momart_SaveCardNumber(CustomerEntity customer, ref string message)
        {
            try
            {
                //customer.CardNumberId = customer.CardNumber;//Rảnh quá
                string TokenToMomart = GetBearerTokenToMomart();
                if (TokenToMomart == null) { message = "Chưa được cấp tài khoản"; return false; }
                else
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToMomart);
                    customer.CreateDate = DateTime.Now;
                    var content2 = new StringContent(new JavaScriptSerializer().Serialize(customer), Encoding.UTF8, "application/json");
                    var resultCardnumbers = client.PostAsync(APIExtension.GetValueAppSettings("API_Momart_SaveCustomerCardNumber"), content2).Result;
                    string jsonCardnumbers = resultCardnumbers.Content.ReadAsStringAsync().Result;
                    int check = int.Parse(JObject.Parse(jsonCardnumbers)["Code"].ToString());
                    //1: dùng được  2: thẻ đã tồn tại, 3:khách hàng đã tồn tại, 4: mã thẻ không tồn tại
                    if (check == 1)
                    {
                        return true;
                    }
                    else
                    {
                        message = JObject.Parse(jsonCardnumbers)["Message"].ToString();
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public bool API_Momart_CheckToInsert(string CardNumberId)
        {
            try
            {
                Log.Info("call API_Momart_CheckToInsert: ");
                //customer.CardNumberId = customer.CardNumber;
                string TokenToMomart = GetBearerTokenToMomart();
                Log.Info("TokenToMomart " + TokenToMomart);
                if (TokenToMomart == null) return false;
                else
                {
                    List<string> listCard = new List<string>();
                    listCard.Add(CardNumberId);
                    Log.Info("count: " + listCard.Count());
                    HttpClient client = new HttpClient();
                    var uri = ConfigurationManager.AppSettings["API_Momart_CheckRegister"].ToString();
                    Log.Info(uri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", TokenToMomart);
                    var content2 = new StringContent(JsonConvert.SerializeObject(listCard), Encoding.UTF8, "application/json");
                    Log.Info(content2);
                    var resultCardnumbers = client.PostAsync(uri, content2).Result;
                    string jsonCardnumbers = resultCardnumbers.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<CardNumbersResult>(jsonCardnumbers);
                    Log.Info("result:" + result);
                    //1: dùng được  2: thẻ đã tồn tại, 3:khách hàng đã tồn tại, 4: mã thẻ không tồn tại
                    if (result.Code == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        #endregion
    }
}
