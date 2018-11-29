using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LIB.Warehouses
{
    public class IplWarehouses : BaseService<WarehousesEntity, long>, IWarehouses
    {
        public List<WarehousesEntity> GetDataWarehouses(bootstrapTableParam obj, ref int totalRecord)
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
                List<WarehousesEntity> datas = unitOfWork.Procedure<WarehousesEntity>("sp_Warehouses_GetAllData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return datas;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
           
        }
        public bool UpdateLogo(string base64Image, int WarehouseId, string PathServer, string PathFile)
        {
            try
            {
                UploadFile.UploadImageWithBase64(base64Image, PathServer, PathFile);
                var param = new DynamicParameters();
                param.Add("@Id", WarehouseId);
                param.Add("@srcLogo", PathFile);
                return unitOfWork.ProcedureExecute("sp_Warehouses_UpdateLogo", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            
        }
        public bool UpdateWarehouse(WarehousesEntity warehouse,string ApisId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", warehouse.Id);
                param.Add("@Name", warehouse.Name);
                param.Add("@Address", warehouse.Address);
                param.Add("@Website", warehouse.Website);
                param.Add("@Phone", warehouse.Phone);
                param.Add("@Hotline", warehouse.Hotline);
                param.Add("@Prefix", warehouse.Prefix);
                param.Add("@QuotaPromotion", warehouse.QuotaPromotion);
                param.Add("@IsSync", warehouse.IsSync);
                param.Add("@ApisId", ApisId);
                param.Add("@Email", warehouse.Email);
                return unitOfWork.ProcedureExecute("sp_Warehouses_Update", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            
        }
        public List<WarehousesEntity> GetAllData()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                return unitOfWork.Procedure<WarehousesEntity>("sp_Warehouses_GetAllDataExist", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<WarehousesEntity> GetAllSync(int IdApi)
        {
            var param = new DynamicParameters();
            param.Add("@IdApi", IdApi);
            var list = unitOfWork.Procedure<WarehousesEntity>("sp_Warehouse_GetAllSync", param).ToList();
            return list;
        }

        public WarehousesEntity CheckSyncWarehouseId(int warehouseId)
        {
            var param = new DynamicParameters();
            param.Add("@warehouseid", warehouseId);
            var data = unitOfWork.Procedure<WarehousesEntity>("SP_Warehouse_CheckSync", param).SingleOrDefault();
            return data;
        }
    }
}
