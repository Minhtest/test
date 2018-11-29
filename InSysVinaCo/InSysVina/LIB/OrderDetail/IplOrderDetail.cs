using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LIB
{
    public class IplOrderDetail : BaseService<OrderDetailEntity, long>, IOrderDetail
    {
        public List<OrderDetailEntity> GetAllByOrderId(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<OrderDetailEntity>("sp_OrderDetail_GetByOrderID", param).ToList();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
        public List<OrderDetailPrint> GetAllByOrderIdPrint(long OrderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                return unitOfWork.Procedure<OrderDetailPrint>("sp_OrderDetail_PrintBill", param).ToList();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
    }
}
