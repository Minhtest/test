using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Orders
{
    public class BillModel
    {
        public List<OrderDetailEntity> listOrderDetail { get; set; }
        public List<OrderPromotionEntity> listProductPromotion { get; set; }
        public CustomerEntity Customer { get; set; }
        public OrderEntity Order { get; set; }

    }
}
