using Dapper;
using Framework.EF;
using LIB.API;
using LIB.CardNumbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LIB
{
    public class IplCustomer : BaseService<CustomerEntity, long>, ICustomer
    {
        private readonly ICardNumbers _cardNumbersService;
        private readonly IAPI _apiService;
        public IplCustomer()
        {
            _cardNumbersService = SingletonIpl.GetInstance<IplCardNumbers>();
            _apiService = SingletonIpl.GetInstance<IplAPI>();
        }
        public List<CustomerEntity> AutoCompleteCustomer(Select2Param obj, ref int total)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@search", obj.term);
                param.Add("@page", obj.page);
                param.Add("@total", DbType.Int32, direction: ParameterDirection.Output);
                List<CustomerEntity> lstCustomer = unitOfWork.Procedure<CustomerEntity>("sp_Customer_GetDataAutoComplete", param).ToList();
                total = param.Get<int>("total");
                return lstCustomer;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public CustomerEntity GetByID(long ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", ID);
                return unitOfWork.Procedure<CustomerEntity>("sp_Customer_GetById", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<CustomerEntity> GetDataCustomer(bootstrapTableParam obj, ref int totalRecord)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                List<CustomerEntity> datas = unitOfWork.Procedure<CustomerEntity>("sp_Customer_GetAllData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return datas;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public string GetNewCode()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Prefix", Enum.PrefixCode.Customer);
                return unitOfWork.Procedure<string>("sp_Customer_CreateCode", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public CustomerEntity GetCustomerByCodeAndCardNumber(CustomerEntity customer)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerCode", customer.CustomerCode);
                //param.Add("@CardNumber", customer.CardNumber);
                CustomerEntity cus = unitOfWork.Procedure<CustomerEntity>("sp_Customer_GetByCodeAndCardNumber", param).SingleOrDefault();
                return cus;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public CustomerEntity Insert_Update(CustomerEntity customer, ref object message)
        {
            try
            {
                try
                {
                    int.Parse(customer.Phone);
                    if (10 > customer.Phone.Length && customer.Phone.Length > 11)
                    {
                        customer.Phone = null;
                    }
                }
                catch (Exception)
                {
                    customer.Phone = null;
                }
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", customer.Id);
                param.Add("@Name", customer.Name);
                param.Add("@Email", customer.Email);
                param.Add("@Phone", customer.Phone);
                param.Add("@Address", customer.Address);
                param.Add("@BirthDay", customer.BirthDay);
                param.Add("@CardNumberId", customer.CardNumberId);
                param.Add("@Prefix", Enum.PrefixCode.Customer);
                CustomerEntity cus = unitOfWork.Procedure<CustomerEntity>("sp_Customer_InsertOrUpdate", param).SingleOrDefault();

                return cus;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                message = (customer.Id == null ? "Thêm khách hàng thất bại! Vui lòng liên hệ kỹ thuật viên." : "Cập nhật khách hàng thất bại! Vui lòng liên hệ kỹ thuật viên.");
                return null;
            }
        }
        public bool DeleteCustomer(long CustomerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", CustomerId);
                return unitOfWork.ProcedureExecute("sp_Customer_Delete", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public bool CustomerCheckExist(CustomerEntity customer, ref object message)
        {
            bool check = false;
            if (CustomerCountExitsByEmail(customer.Id, customer.Email) > 0)
            {
                message += "* Email khách hàng đã tồn tại!";
                check = true;
            }
            if (CustomerCountExitsByPhone(customer.Id, customer.Phone) > 0)
            {
                message += "* Số điện thoại khách hàng đã tồn tại!";
                check = true;
            }
            return check;
        }
        public int CustomerCountExitsByCardNumber(long? CustomerId, string CardNumber)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", CustomerId);
                param.Add("@CardNumber", CardNumber);
                return unitOfWork.Procedure<int>("sp_Customer_CountExitsByCardNumber", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public int CustomerCountExitsByEmail(long? CustomerId, string Email)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", CustomerId);
                param.Add("@Email", Email);
                return unitOfWork.Procedure<int>("sp_Customer_CountExitsByEmail", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public int CustomerCountExitsByPhone(long? CustomerId, string Phone)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", CustomerId);
                param.Add("@Phone", Phone);
                return unitOfWork.Procedure<int>("sp_Customer_CountExitsByPhone", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public CustomerEntity GetCustomerByCardNumberId(string CardNumberId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CardNumberId", CardNumberId);
                return unitOfWork.Procedure<CustomerEntity>("sp_Customer_GetByCardNumberId", param).SingleOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
