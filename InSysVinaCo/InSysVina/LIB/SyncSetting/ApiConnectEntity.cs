using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.SyncSetting
{
    public class ApiConnectEntity
    {
        
        public int Id { get; set; }
        [DisplayName("Tên")]
        public string Name { get; set; }
        public string Secret { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Token { get; set; }
        [DisplayName("Địa chỉ")]
        public string Api { get; set; }
       
        public bool Disabled { get; set; }
        public string Type { get; set; }
        public string Client { get; set; }
    }
}
