using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Model
{
    [Table("Right")]
    public class RightEntity : BaseEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int ModuleId { get; set; }
        public int Sorting { get; set; }
    }

    [Table("RoleMapRight")]
    public class RoleMapRightEntity : BaseEntity<int>
    {
        public int RoleId { get; set; }
        public int RightId { get; set; }
        public string RightCode { get; set; }
    }
}
