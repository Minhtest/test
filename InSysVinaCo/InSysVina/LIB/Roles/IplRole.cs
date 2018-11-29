using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using LIB.Model;
using LIB.RoleModule;

namespace LIB
{
    public class IplRole : BaseService<RoleEntity, int>, IRole
    {
        public IplRole() { }

        public List<RightEntity> GetRightByRole(int RoleId)
        {
            return GetRightByRole(RoleId.ToString());
        }

        public List<RightEntity> GetRightByRole(string RoleIds)
        {
            return this.Raw_Query<RightEntity>(@"
                select
	                r.*
                from [Right] r
                inner join RoleMapRight rmr on rmr.RightCode = r.Code
                inner join dbo.Split(@RoleIds, ',') temp on temp.ID = rmr.RoleId
            ", param: new Dictionary<string, object>() {
                { "RoleIds", RoleIds}
            }).ToList();
        }

        public List<RightEntity> GetRightByUserIds(int UserId)
        {
            return GetRightByUserIds(UserId.ToString());
        }

        public List<RightEntity> GetRightByUserIds(string UserIds)
        {
            return this.Raw_Query<RightEntity>(@"
                select
	                r.*
                from [Right] r
                inner join RoleMapRight rmr on rmr.RightCode = r.Code
                inner join Users u on u.RoleId = rmr.RoleId
                inner join dbo.Split(@UserIds, ',') temp on temp.ID = u.Id
            ", param: new Dictionary<string, object>() {
                { "UserIds", UserIds}
            }).ToList();
        }

        public List<RoleModuleEntity> GetRoleModuleByRole(string roleIds)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@roleIds", roleIds);
                return unitOfWork.Procedure<RoleModuleEntity>("sp_RoleModule_GetByRole1", p).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

        }
        public List<RoleModuleEntity> GetRoleModuleByUserIds(string UserIds)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@UserId", UserIds);
                return unitOfWork.Procedure<RoleModuleEntity>("sp_RoleModule_GetByUserID", p).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }

        public bool UpdateRight(int roleId, string xml)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@roleId", roleId);
                p.Add("@xml", xml);
                return unitOfWork.ProcedureExecute("sp_RoleMapRight_Update", p);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public List<RoleEntity> UserInRole(int userId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@UserId", userId);
                return unitOfWork.Procedure<RoleEntity>("sp_UserInRole_GetByUserId", p).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
        public List<RoleEntity> GetRolesByLevel(int level)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Level", level);
                return unitOfWork.Procedure<RoleEntity>("sp_Roles_GetDataByLevel", param).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }
    }
}
