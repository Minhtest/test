using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.SyncSetting
{
    public interface IApiConnect : IBaseServices<ApiConnectEntity, long>
    {
        List<ApiConnectEntity> GetData(bootstrapTableParam obj, ref int totalRecord);
        bool UpdateApiConnect(ApiConnectEntity ApiConnect);
        ApiConnectEntity GetDetail(int ApiConnect);
        bool DeleteApiConnect(int Id);
        List<ApiConnectEntity> GetSelect2Data();
        List<ApiConnectEntity> GetByWarehouseId(int GetByWarehouseId);
    }
    public class IplApiConnect: BaseService<ApiConnectEntity, long>, IApiConnect
    {
        public List<ApiConnectEntity> GetData(bootstrapTableParam obj, ref int totalRecord)
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
                List<ApiConnectEntity> datas = unitOfWork.Procedure<ApiConnectEntity>("sp_ApiConnect_GetAllData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return datas;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<ApiConnectEntity> GetByWarehouseId(int GetByWarehouseId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@WarehouseId", GetByWarehouseId);
                List<ApiConnectEntity> datas = unitOfWork.Procedure<ApiConnectEntity>("sp_ApiConnect_GetByWarehoueId", param).ToList();
                return datas;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<ApiConnectEntity> GetSelect2Data()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                List<ApiConnectEntity> datas = unitOfWork.Procedure<ApiConnectEntity>("sp_ApiConnect_GetAll", param).ToList();
                return datas;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public bool UpdateApiConnect(ApiConnectEntity ApiConnect)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ApiConnect.Id);
                param.Add("@Name", ApiConnect.Name);
                param.Add("@Secret", ApiConnect.Secret);
                param.Add("@UserName", ApiConnect.UserName);
                param.Add("@PassWord", ApiConnect.PassWord);
                param.Add("@Token", ApiConnect.Token);
                param.Add("@Api", ApiConnect.Api);
                param.Add("@Disabled", ApiConnect.Disabled);
                param.Add("@Type", ApiConnect.Type);
                param.Add("@Client", ApiConnect.Client);
                return unitOfWork.ProcedureExecute("sp_ApiConnect_CreateUpdate", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

        }
        public ApiConnectEntity GetDetail(int IdApiConnect)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", IdApiConnect); 
                return unitOfWork.Procedure<ApiConnectEntity>("sp_ApiConnect_GetDetail", param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public bool DeleteApiConnect(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                return unitOfWork.ProcedureExecute("sp_ApiConnect_Delete", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }
    }
}
