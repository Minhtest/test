using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Orders
{
    public class OrderReportModel
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public decimal ProductTotal { get; set; }
        public decimal? Discount { get; set; }
        public int? PointUsed { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal? PayCash { get; set; }
        public decimal? PayByCard { get; set; }
        public decimal RefundMoney { get; set; }
        public int? ReturnId { get; set; }
        public decimal? TotalPriceReturn { get; set; }
    }
}
