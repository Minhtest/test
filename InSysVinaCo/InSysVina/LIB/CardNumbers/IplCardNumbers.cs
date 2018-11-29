using Dapper;
using LIB.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LIB.CardNumbers
{
    public interface ICardNumbers : IBaseServices<CardNumbersEntity, int>
    {
        string GetCardNumber();
        //List<CardNumbersEntity> ListCardNumber();
        /// <summary>
        /// Get all data with status = 1
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
        List<CardNumbersEntity> GetAllData(bootstrapTableParam obj, ref int totalRecord);
        List<CardNumbersEntity> GetDataExist();
        ResultMessageModel Sync();
        bool CheckExist(string CardNumberId);
        CardNumbersEntity CheckCardUse(string CardNumberId);
        List<CardNumbersEntity> AutoCompleteCardNumber(Select2Param obj, ref int total);
        List<CardNumbersExcel> ExportExcel(bootstrapTableParam obj);
        bool InsertCardNumber(string obj, ref string message);
        bool CheckExistCode(string obj);
        bool CheckRegister(string CardNumberId);
        bool CheckToInsert(string CardNumberId, long? IdCustomer, ref object message);
        CardNumbersEntity GetDetail(string CardNumberId);
        bool CheckToUse(string CardNumberId);
        List<CardNumbersEntity> CardNumber_AutoComplete(Select2Param obj);
    }
    public class IplCardNumbers : BaseService<CardNumbersEntity, int>, ICardNumbers
    {
        public string GetCardNumber()
        {
            string cardnumber = "";
            try
            {
                DynamicParameters param = new DynamicParameters();
                return unitOfWork.Procedure<string>("sp_CardNumbers_GetOneCardNumber", param).SingleOrDefault();
                //while (true)
                //{
                //    //lấy 1 cái thẻ
                //    DynamicParameters param = new DynamicParameters();
                //    cardnumber = unitOfWork.Procedure<string>("sp_CardNumbers_GetOneCardNumber", param).SingleOrDefault();
                //    // nếu hết thẻ trong bảng
                //    if (cardnumber == null || cardnumber == "")
                //    {
                //        IAPI syncDataService = SingletonIpl.GetInstance<IplAPI>();
                //        //lấy thêm bên momart
                //        List<CardNumbersEntity> listCardnumbers = syncDataService.APIGetListCardNumbers(15);
                //        if (listCardnumbers == null)
                //        {
                //            //hết thẻ
                //            break;
                //        }
                //        else
                //        {
                //            //insert vào bảng cardnumber
                //            string xmlCardNumbers = XMLHelper.SerializeXML<List<CardNumbersEntity>>(listCardnumbers).Replace("xsi:nil=\"true\"","");
                //            DynamicParameters paramXml = new DynamicParameters();
                //            paramXml.Add("@strXml", xmlCardNumbers);
                //            unitOfWork.ProcedureExecute("sp_CardNumbers_Insert", paramXml); 
                //        }
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                //return cardnumber;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return "";
            }
        }
        //public r ListCardNumber()
        //{
        //    var api = new IplAPI();
        //    return api.API_Momart_GetCardNumbers();
        //}
        public List<CardNumbersEntity> GetAllData(bootstrapTableParam obj, ref int totalRecord)
        {
            try
            {
                try
                {
                    var param = new DynamicParameters();
                    param.Add("@txtSearch", obj.search);
                    param.Add("@pageNumber", obj.pageNumber());
                    param.Add("@pageSize", obj.pageSize());
                    param.Add("@order", obj.order);
                    param.Add("@sort", obj.sort);
                    param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var data = unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumbers_GetAllData", param).ToList();
                    totalRecord = param.Get<int>("@totalRecord");
                    return data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<CardNumbersExcel> ExportExcel(bootstrapTableParam obj)
        {
            try
            {
                try
                {
                    var param = new DynamicParameters();
                    param.Add("@txtSearch", obj.search);
                    param.Add("@order", obj.order);
                    param.Add("@sort", obj.sort);
                    var data = unitOfWork.Procedure<CardNumbersExcel>("sp_CardNumbers_ExportExcel", param).ToList();

                    return data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<CardNumbersEntity> GetDataExist()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                return unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumbers_GetDataExist", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public ResultMessageModel Sync()
        {
            try
            {
                var api = new IplAPI();
                ResultMessageModel result = api.API_Momart_GetCardNumbers();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ResultMessageModel("Đồng bộ thất bại. Có lỗi xảy ra!", 3); ;
            }
        }
        public bool CheckExist(string CardNumberId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                int count = unitOfWork.Procedure<int>("sp_CardNumbers_CheckExist", param).SingleOrDefault();
                if (count > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

        }
        public CardNumbersEntity CheckCardUse(string CardNumberId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@cardnumberid", CardNumberId);
                var res = unitOfWork.Procedure<CardNumbersEntity>("SP_CardNumber_CheckUse", p).SingleOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<CardNumbersEntity> AutoCompleteCardNumber(Select2Param obj, ref int total)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@search", obj.term);
                param.Add("@page", obj.page);
                param.Add("@total", DbType.Int32, direction: ParameterDirection.Output);
                List<CardNumbersEntity> lstCardNumber = unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumber_GetDataAutoComplete", param).ToList();
                total = param.Get<int>("total");
                return lstCardNumber;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<CardNumbersEntity> CardNumber_AutoComplete(Select2Param obj)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Search", obj.term);
                List<CardNumbersEntity> lstCardNumber = unitOfWork.Procedure<CardNumbersEntity>("spCardNumbers_AutoComplete", param).ToList();
                return lstCardNumber;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public bool InsertCardNumber(string CardNumberId, ref string message)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                CardNumbersEntity lstCardNumber = unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumber_Insert", param).SingleOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                message = ex.Message;
                return false;
            }
        }
        public bool CheckExistCode(string CardNumberId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                if (unitOfWork.Procedure<int>("sp_CardNumber_CheckExitstCode", param).SingleOrDefault() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public bool CheckRegister(string CardNumberId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                if (unitOfWork.Procedure<int>("sp_CardNumber_CheckRegister", param).SingleOrDefault() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public bool CheckToInsert(string CardNumberId, long? IdCustomer, ref object message)
        {
            try
            {
                var api = new IplAPI();
                Log.Info("Card number:" + CardNumberId);
                var CardNumber = GetDetail(CardNumberId);
                if(CardNumber != null) // có trong shop
                {
                    if(CardNumber.IsVerify != null) // đã verify
                    {
                        if (CardNumber.IsVerify == true && (CardNumber.CustomerId == null || ( IdCustomer != null && CardNumber.CustomerId == IdCustomer))) // đã xác nhận và chưa có khách hàng đký
                        {
                            message = "Thành công!";
                            return true;
                        }
                        else
                        {
                            message = "Thẻ không thể sử dụng";
                            return false;
                        }
                    }
                    else // chưa verify
                    { 
                        bool res = api.API_Momart_CheckToInsert(CardNumberId); // card thoả mãn
                        if (res)
                        {
                            var param = new DynamicParameters();
                            param.Add("@CardNumberId", CardNumberId);
                            unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumber_UpdateVerifyOne", param);
                            return true;
                        }
                        else
                        {
                            message = "Thẻ không thể sử dụng";
                            return false;
                        }
                    }
                }
                else //chưa có trong shop
                {
                    bool res = api.API_Momart_CheckToInsert(CardNumberId); // card thoả mãn
                    if (res)
                    {
                        var param = new DynamicParameters();
                        param.Add("@CardNumberId", CardNumberId);
                        CardNumbersEntity lstCardNumber = unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumber_Insert_Verify", param).SingleOrDefault();
                        return true;
                    }
                    else
                    {
                        message = "Thẻ không thể sử dụng";
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
        public CardNumbersEntity GetDetail(string CardNumberId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                return unitOfWork.Procedure<CardNumbersEntity>("sp_CardNumbers_GetDetail", param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public bool CheckToUse(string CardNumberId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@cardNumberId", CardNumberId);
                if (unitOfWork.Procedure<int>("SP_CardNumber_CheckToUse", param).SingleOrDefault() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
    }

}
