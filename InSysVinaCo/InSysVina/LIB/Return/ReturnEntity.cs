using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace LIB
{
    public class ReturnEntity
    {
        public int Id { get; set; }
        public long OrderId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool isActive { get; set; }
        public string CustomerName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime OrderDate { get; set; }
        public int WarehouseId { get; set; }
        public string OrderCode { get; set; }
        public int IsDel { get; set; }
    }
    public class ReturnDetailEntity
    {
        public int? Id { get; set; }
        public int? ReturnId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
        public decimal SellPrice { get; set; }
        public float Discount { get; set; }
        public int? QuantityReturn { get; set; }
        public decimal? PriceReturn { get; set; }
        public string Reason { get; set; }
    }
}
