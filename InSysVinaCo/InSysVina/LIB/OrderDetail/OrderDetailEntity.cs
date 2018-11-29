using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace LIB
{
    [Table("OrderDetail")]
    public class OrderDetailEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int InventoryNumber { get; set; }
        public long ParentId { get; set; }

        public decimal Price { get; set; }

        public decimal SellPrice { get; set; }
        public decimal Discount { get; set; }
        [XmlElement(IsNullable = false)]
        public decimal Coupon { get; set; }
        public string CouponCode { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class OrderDetailPrint
    {
        #region Properties

        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Quantity { get; set; }
        public int InventoryNumber { get; set; }
        public long ParentId { get; set; }
        public string Price { get; set; }
        public string SellPrice { get; set; }
        public string Total { get; set; }
        public string Discount { get; set; }
        public decimal Coupon { get; set; }
        public string CouponCode { get; set; }

        #endregion
    }
}
