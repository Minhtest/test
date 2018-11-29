using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LIB.ExcelExtension;

namespace LIB
{
    [Table("Customer")]
    public class CustomerEntity
    {
        public long? Id { get; set; }

        [DisplayName("Tên Khách Hàng")]
        [ExcelColumn(ColumnName: "A")]
        public string Name { get; set; }

        [DisplayName("Mã Thẻ")]
        [ExcelColumn(ColumnName: "B")]
        public string CardNumberId { get; set; }



        [ExcelColumn(ColumnName: "C")]
        [DisplayName("Ngày Sinh")]
        public DateTime? BirthDay { get; set; }


        [DisplayName("Số Điện Thoại")]
        [ExcelColumn(ColumnName: "D")]
        public string Phone { get; set; }



        [DisplayName("Địa Chỉ")]
        [ExcelColumn(ColumnName: "E")]
        public string Address { get; set; }


        [DisplayName("Email Khách Hàng")]
        [ExcelColumn(ColumnName: "F")]
        public string Email { get; set; }

        [DisplayName("Mã Khách Hàng")]
        public string CustomerCode { get; set; }

        public int Point { get; set; }

        public int CountBuy { get; set; }

        public decimal TotalBuy { get; set; }

        public decimal MoneyAllPoint { get; set; }

        public bool AllowDelete { get; set; }

        public DateTime CreateDate { get; set; }

        public string ImportMessage { get; set; }
    }
}
