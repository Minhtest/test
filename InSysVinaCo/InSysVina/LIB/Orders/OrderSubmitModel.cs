using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LIB.Orders
{
    public class OrderSubmitModel
    {
        public OrderEntity Order { get; set; }
        public List<OrderDetailEntity> OrderDetail { get; set; }
        public List<OrderPromotionEntity> OrderPromotion { get; set; }
    }
    public class OrderShop
    {
        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập mã order")]
        [MaxLength(50)]
        public string OrderResource { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập mã kho")]
        public string WarehouseCode { get; set; }
        public string Note { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập TotalPrice")]
        public double TotalPrice { get; set; }
        public string TypeOrder { get; set; }
        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập sản phẩm")]
        public List<OrderItem> OrderItems { get; set; }
    }
    public class OrderPost
    {
        public List<OrderShop> OrderShops { get; set; }
    }
    public class OrderItem
    {
        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập mã Barcode")]
        [MaxLength(50)]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập mã ProductCode")]
        [MaxLength(50)]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập Price")]
        public double Price { get; set; }
        public string Type { get; set; }
        public double? Discount { get; set; }
        public double? PercentDiscount { get; set; }

        [Required(ErrorMessage = "Yêu cầu bắt buộc nhập TotalPrice")]
        public double TotalPrice { get; set; }
    }
    public class OrderResult
    {
        public string OrderResource { get; set; }
        public string WarehouseCode { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
    }
    public class OrderResponse
    {
        public bool Status { get; set; }
        public List<OrderResult> OrderResults { get; set; }
    }
}
