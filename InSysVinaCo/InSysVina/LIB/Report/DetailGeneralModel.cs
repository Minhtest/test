using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Report
{
    public class DetailGeneralModel
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string CreatedBy_Name { get; set; }
        public string CreatedBy_UserName { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CardNumber { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ComputeUnit { get; set; }
        public int Quantity { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Discount_Product { get; set; }
        public decimal ProductTotal { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public int PointUsed { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidGuests { get; set; }
        public decimal RefundMoney { get; set; }
    }
}
