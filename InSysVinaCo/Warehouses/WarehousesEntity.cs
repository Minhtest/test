using System;
using Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIB.Warehouses
{
    [Table("Warehouses")]
    public class WarehousesEntity
    {
        #region Properties
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên kho")]
        [MaxLength(500)]
        [Display(Name = "Tên kho")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Chưa nhập địa chỉ")]
        [MaxLength(500)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(150)]
        public string Phone { get; set; }

        [Display(Name = "Hotline")]
        [MaxLength(150)]
        public string Hotline { get; set; }

        [Display(Name = "Website")]
        [MaxLength(550)]
        public string Website { get; set; }


        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        #endregion
    }
}
