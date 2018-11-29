using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LIB
{
    public class IplReturn : BaseService<ReturnEntity, long>, IReturn
    {
        public List<ReturnEntity> GetData(bootstrapTableParam obj,int? WarehouseId, ref int totalRecord)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@WarehouseId", WarehouseId);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var data = unitOfWork.Procedure<ReturnEntity>("sp_Return_GetData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public bool InsertOrUpdate(int? ReturnId, long OrderId, int UserId, int WarehouseId, List<ReturnDetailEntity> list)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                string strXML = XMLHelper.SerializeXML<List<ReturnDetailEntity>>(list).Replace("xsi:nil=\"true\"", "").ToString();
                param.Add("@ReturnId", ReturnId);
                param.Add("@OrderId", OrderId);
                param.Add("@UserId", UserId);
                param.Add("@WarehouseId", WarehouseId);
                param.Add("@xml", strXML);
                return unitOfWork.ProcedureExecute("sp_Return_InsertOrUpdate", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
        public ReturnEntity GetReturnById(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                return unitOfWork.Procedure<ReturnEntity>("sp_Return_GetById", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<ReturnDetailEntity> GetReturnDetailByOrderId(int OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<ReturnDetailEntity>("sp_ReturnDetail_GetByOrderId", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<ReturnDetailEntity> GetReturnDetail(int ReturnId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ReturnId", ReturnId);
                return unitOfWork.Procedure<ReturnDetailEntity>("sp_ReturnDetail_ReturnId", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<OrderEntity> GetDataOrders(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@txtSearch", obj.search);
                param.Add("@WarehouseId", WarehouseId);
                param.Add("@pageNumber", obj.pageNumber());
                param.Add("@pageSize", obj.pageSize());
                param.Add("@order", obj.order);
                param.Add("@sort", obj.sort);
                param.Add("@totalRecord", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var data = unitOfWork.Procedure<OrderEntity>("sp_OrdersReturn_GetData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public ReturnEntity GetByOrderId_WarehouseId(long OrderId, int WarehouseId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                param.Add("@WarehouseId", WarehouseId);
                return unitOfWork.Procedure<ReturnEntity>("sp_Return_GetByOrderId_WarehouseId", param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                return unitOfWork.ProcedureExecute("sp_Return_Delete", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
    }
}
