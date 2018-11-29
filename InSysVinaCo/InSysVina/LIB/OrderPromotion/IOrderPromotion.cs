using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB
{
	public interface IOrderPromotion : IBaseServices<OrderPromotionEntity, long>
    {
        List<OrderEntity> GetData(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord);
        List<OrderEntity> GetPaidOrders(bootstrapTableParam obj, int? WarehouseId, ref int totalRecord);
        List<OrderPromotionEntity> GetAllByOrderId(long OrderId);
        bool UpdatePromotionOrder(long OrderId, List<OrderPromotionEntity> list, byte isTang, ref string message);
        bool DeletePromotionOrder(long OrderId);
    }
}
