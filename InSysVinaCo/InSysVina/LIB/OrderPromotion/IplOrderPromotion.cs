using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LIB
{
    public class IplOrderPromotion : BaseService<OrderPromotionEntity, long>, IOrderPromotion
    {
        public List<OrderEntity> GetData(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord)
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
                var data = unitOfWork.Procedure<OrderEntity>("sp_OrdersPromotion_GetData", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<OrderEntity> GetPaidOrders(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord)
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
                var data = unitOfWork.Procedure<OrderEntity>("sp_OrdersPromotion_GetDataPaid", param).ToList();
                totalRecord = param.Get<int>("@totalRecord");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
        public bool UpdatePromotionOrder(long OrderId, List<OrderPromotionEntity> list, byte isTang, ref string message)
        {
            try {
                DynamicParameters param = new DynamicParameters();
                long validate = 0;
                string strXML = XMLHelper.SerializeXML<List<OrderPromotionEntity>>(list).Replace("xsi:nil=\"true\"", "").ToString();
                param.Add("@OrderId", OrderId);
                param.Add("@xml", strXML);
                param.Add("@isTang", isTang);
                param.Add("@Validate", validate, dbType: DbType.Int64, direction: ParameterDirection.Output);
                var kq = unitOfWork.ProcedureExecute("sp_OrdersPromotion_Update", param);
                validate = param.Get<long>("@Validate");
                if (validate > 0)
                {
                    message = "Số lượng sản phẩm không đủ!";
                    return false;
                }
                return kq;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

        }
        public bool DeletePromotionOrder(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.ProcedureExecute("sp_OrdersPromotion_Delete", param);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

        }
        public List<OrderPromotionEntity> GetAllByOrderId(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<OrderPromotionEntity>("sp_OrdersPromotion_GetByOrderID", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
    }
}
