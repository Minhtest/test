using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Warehouses
{
    public class IplWarehouses : BaseService<WarehousesEntity, long>, IWarehouses
    {
       
    }
}
