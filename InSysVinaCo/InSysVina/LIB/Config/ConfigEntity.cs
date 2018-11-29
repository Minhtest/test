using System;
//using Framework.Extensions;
using Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Framework.Data.Attributes;
using System.ComponentModel;

namespace LIB
{
    [Table("Config")]
    public class ConfigEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int NumValue { get; set; }
        public string TextValue { get; set; }
    }
}