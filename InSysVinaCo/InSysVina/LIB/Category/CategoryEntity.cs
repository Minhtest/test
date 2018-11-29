using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LIB.ExcelExtension;

namespace LIB
{
    [Table("Category")]
    public class CategoryEntity : CategoryEntityMore
    {
        #region Properties
        public int? Id { get; set; }
        [DisplayName("Mã danh mục")]
        [ExcelColumn(ColumnName: "A")]
        public string Code { get; set; }
        [DisplayName("Tên danh mục")]
        [ExcelColumn(ColumnName: "B")]
        public string Name { get; set; }
        [DisplayName("Danh mục cha")]
        public int? ParentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [DefaultValue(true)]
        public bool isActive { get; set; }
        public string ImportSuccess { get; set; }
        public string ImportMessage { get; set; }
        #endregion
    }
    public class CategoryEntityMore
    {
        public int Level { get; set; }
    }
}
