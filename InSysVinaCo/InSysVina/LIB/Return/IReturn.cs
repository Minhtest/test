using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB
{
    public interface IReturn : IBaseServices<ReturnEntity, long>
    {
        bool InsertOrUpdate(int? ReturnId, long OrderId, int UserId, int WarehouseId, List<ReturnDetailEntity> list);
        List<ReturnEntity> GetData(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord);
        ReturnEntity GetReturnById(int Id);
        List<ReturnDetailEntity> GetReturnDetailByOrderId(int OrderId);
        ReturnEntity GetByOrderId_WarehouseId(long OrderId, int WarehouseId);
        List<OrderEntity> GetDataOrders(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord);
        bool Delete(int Id);
        List<ReturnDetailEntity> GetReturnDetail(int ReturnId);
    }
}
