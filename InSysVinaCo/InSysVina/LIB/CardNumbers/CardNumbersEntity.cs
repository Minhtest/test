using System;
using System.ComponentModel;
using static LIB.ExcelExtensionCustom;

namespace LIB.CardNumbers
{
    public class CardNumbersEntity:BaseEntity<int>
    {
        [DisplayName("Mã thẻ")]
        [ExcelColumnCustom(ColumnName: "A")]
        public string CardNumberId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int TotalPoint { get; set; }
        public int UsePoint { get; set; }
        public int Point { get; set; }
        public DateTime? TimeSync { get; set; } 
        public byte IsDelCustomer { get; set; }
        public bool IsActive { get; set; }
        public bool? IsVerify { get; set; }
        public string ImportSuccess { get; set; }
    }
    public class CardNumbersExcel
    {
        public string CardNumberId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int TotalPoint { get; set; }
        public int UsePoint { get; set; }
        public int Point { get; set; }
        public string  Status { get; set; }
    }
}
