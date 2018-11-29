using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace LIB
{
    [Table("Orders")]
    public class OrderEntity : OrderEntityMore
    {
        [DisplayName("Id")]
        public long? Id { get; set; }
        public string OrderCode { get; set; }
        [DisplayName("CustomerId")]
        public long? CustomerId { get; set; }
        [DisplayName("Coupon")]
        public string CouponCode { get; set; }
        public decimal? CouponValue { get; set; }
        /// <summary>
        /// 1: Trừ trực tiếp
        /// 2: Trừ theo %
        /// </summary>
        public int? CouponType { get; set; }
        [DisplayName("Voucher")]
        public decimal? Voucher { get; set; }
        [DisplayName("Tiền giảm")]
        public decimal Discount { get; set; }
        public float? PercentForPoint { get; set; }
        public decimal? MoneyOnePoint { get; set; }
        public int? PointUsed { get; set; }
        [DisplayName("Thành tiền")]
        public decimal ProductTotal { get; set; }
        [DisplayName("Tổng tiền")]
        public decimal GrandTotal { get; set; }
        [DisplayName("Thời gian Order")]
        public DateTime? OrderDate { get; set; }
        public int? WarehouseId { get; set; }
        [DisplayName("Bằng tiền mặt")]
        public decimal? PayCash { get; set; }
        [DisplayName("Bằng thẻ")]
        public decimal? PayByCard { get; set; }
        [DisplayName("Tiền trả lại khách")]
        public decimal RefundMoney { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        [DisplayName("Người tạo")]
        public int? CreatedBy { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Người sửa")]
        public int? ModifiedBy { get; set; }
        [DisplayName("Ngày sửa")]
        public DateTime? ModifiedDate { get; set; }
    }
    public class OrderEntityMore
    {
        public string CustomerCode { get; set; }
        [DisplayName("Tên khách hàng")]
        public string CustomerName { get; set; }
        [DisplayName("Email khách hàng")]
        public string Email { get; set; }
        [DisplayName("Điện thoại khách hàng")]
        public string Phone { get; set; }
        [DisplayName("Địa chỉ khách hàng")]
        public string Address { get; set; }
        [DisplayName("Mã người tạo")]
        public string CreatedByCode { get; set; }
        [DisplayName("Mã người sửa")]
        public string ModifiedByCode { get; set; }
        public bool UsePoint { get; set; }
        public DateTime? PromotedDate { get; set; }
        public DateTime? DateSync { get; set; }
    }

    public class OrderPrint
    {
       
        public long? Id { get; set; }
        public string Code { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Company { get; set; }
        public string Nodes { get; set; }
        public string Discount { get; set; }
        public int CouponId { get; set; }
        public string ProductTotal { get; set; }
        public string GrandTotal { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long WarehouseId { get; set; }
        public string PaidGuests { get; set; }
        public string RefundMoney { get; set; }
        public int? DiscountType { get; set; }
        public string CustomerCode { get; set; }
        public decimal? MoneyUsePoint { get; set; }
        public string CouponCode { get; set; }
        public string Amttotal { get; set; }

    }
    public class Orders_GetRevenueModel
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal Revenue { get; set; }
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
