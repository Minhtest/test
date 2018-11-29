using LIB.Model;
using System;
using System.Collections.Generic;

namespace LIB
{
    public interface IRight : IBaseServices<RightEntity, int>
    {
        bool UpdateModuleIdAndSort(string xml);
    }
    public interface IRoleMapRight : IBaseServices<RoleMapRightEntity, int>
    {
        bool UpdateRoleMapRight(IEnumerable<RoleMapRightEntity> data, int RoleId);
    }
}
