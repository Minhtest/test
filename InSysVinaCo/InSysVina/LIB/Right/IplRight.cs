using System.Collections.Generic;
using System.Linq;
using LIB.Model;

namespace LIB
{
    public class IplRight : BaseService<RightEntity, int>, IRight
    {
        public IplRight() { }

        public bool UpdateModuleIdAndSort(string xml)
        {
            var result = this.unitOfWork.Procedure<int>("sp_Right_UpdateModuleIdAndSort", new
            {
                xml = xml
            });

            return true;
        }
    }
    public class IplRoleMapRight : BaseService<RoleMapRightEntity, int>, IRoleMapRight
    {
        public IplRoleMapRight() { }


        public bool UpdateRoleMapRight(IEnumerable<RoleMapRightEntity> data, int RoleId)
        {
            this.Raw_DeleteStringCustom("where RoleId = " + RoleId, null);
            if (data.Count() > 0)
            {
                this.Raw_InsertAll(data.ToList(), new List<string>()
                {
                    "RoleId",
                    "RightCode",
                });
            }
            return true;
        }
    }
}
