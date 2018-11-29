using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Warehouses
{
    public interface IWarehouses : IBaseServices<WarehousesEntity, long>
    {
        bool UpdateWarehouse(WarehousesEntity warehouse, string ApisId);
        bool UpdateLogo(string base64Image, int WarehouseId,string PathServer, string PathFile);
        List<WarehousesEntity> GetDataWarehouses(bootstrapTableParam obj, ref int totalRecord);
        List<WarehousesEntity> GetAllData();
        List<WarehousesEntity> GetAllSync(int IdApi);
        WarehousesEntity CheckSyncWarehouseId(int warehouseId);
    }
}
