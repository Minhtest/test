using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace LIB
{
    [Table("OrderPromotion")]
    public class OrderPromotionEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int InventoryNumber { get; set; }
    }
}
