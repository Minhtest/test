using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Tokens
{
    public class ClientEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string SecretName { get; set; }
        public string SecretValue { get; set; }
        public string SecretDisplay { get; set; }
        public int LifeTime { get; set; }
        public string Type { get; set; }
        public int WarehouseId { get; set; }
        public bool isActive { get; set; }
    }
}
