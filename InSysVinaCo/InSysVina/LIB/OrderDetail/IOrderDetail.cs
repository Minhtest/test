using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB
{
	public interface IOrderDetail: IBaseServices<OrderDetailEntity, long>
    {
        /// <summary>
        /// Lấy chi tiết Hóa Đơn theo OrderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        List<OrderDetailEntity> GetAllByOrderId(long OrderId);
        List<OrderDetailPrint> GetAllByOrderIdPrint(long OrderId);
    }
}
