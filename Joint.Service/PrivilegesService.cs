using Joint.Entity;
using Joint.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Service
{
    public partial class PrivilegesService : BaseService<Privileges>, IPrivilegesService
    {
        public bool HasSystemPrivileges(int SystemID, string code)
        {
            return new RelationPrivilegesSystemService().Exists(t => t.SystemID == SystemID && t.Privileges.Code == code);
        }

        public bool HasShopsPrivileges(int ShopsID, string code)
        {
            return new RelationPrivilegesShopsService().Exists(t => t.ShopsID == ShopsID && t.Privileges.Code == code);
        }

        public bool HasStoresPrivileges(int StoresID, string code)
        {
            return new RelationPrivilegesStoresService().Exists(t => t.StoresID == StoresID && t.Privileges.Code == code);
        }

        public bool HasUsersPrivileges(int UsersID, string code)
        {
            return new RelationPrivilegesUsersService().Exists(t => t.UsersID == UsersID && t.Privileges.Code == code);
        }
    }
}
