using Joint.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.IService
{
    public partial interface IPrivilegesService : IBaseService<Privileges>
    {
        bool HasSystemPrivileges(int SystemID, string code);

        bool HasShopsPrivileges(int ShopsID, string code);

        bool HasStoresPrivileges(int StoresID, string code);

        bool HasUsersPrivileges(int UsersID, string code);
    }
}
