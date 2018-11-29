using Dapper;
using System;
using System.Linq;

namespace LIB
{
    public class IplConfig : BaseService<ConfigEntity, int>, IConfig
    {
        public IplConfig() { }
    }
}
