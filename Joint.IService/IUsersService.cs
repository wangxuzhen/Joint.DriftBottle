
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Joint.Entity;

namespace Joint.IService
{
    public partial interface IUsersService : IBaseService<Users>
    {
        Users Login(string userName, string password);


        /// <summary>
        /// 获取一个用户的所有角色
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string GetUserAllRole(int userID);


    }

}

