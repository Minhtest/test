using System.Collections.Generic;

namespace LIB
{
    public interface ICustomer : IBaseServices<CustomerEntity, long>
    {
        List<CustomerEntity> AutoCompleteCustomer(Select2Param obj,ref int total);
        /// <summary>
        /// Lấy khách hàng bằng id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        CustomerEntity GetByID(long ID);
        List<CustomerEntity> GetDataCustomer(bootstrapTableParam obj, ref int totalRecord);
        string GetNewCode();
        CustomerEntity GetCustomerByCodeAndCardNumber(CustomerEntity customer);
        CustomerEntity Insert_Update(CustomerEntity customer, ref object message);
        bool DeleteCustomer(long CustomerId);
        bool CustomerCheckExist(CustomerEntity customer, ref object message);
        CustomerEntity GetCustomerByCardNumberId(string CardNumberId);
    }
}
