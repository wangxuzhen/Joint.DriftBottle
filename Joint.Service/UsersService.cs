using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Joint.IService;
using Joint.IRepository;
using Joint.Entity;
using Joint.DLLFactory;
using Joint.Repository;
using System.Collections.Generic;

namespace Joint.Service
{
    public partial class UsersService : BaseService<Users>, IUsersService
    {
        public Users Login(string userName, string password)
        {
            var user = currentRepository.GetFirstOrDefault(t => t.UserName == userName);
            if (user == null)
            {
                return null;
            }

            //判断密码是否正确
            string endPassword = password + user.PasswordSalt;
            string MD5Pwd = Common.SecureHelper.MD5(endPassword);
            if (user.Password == MD5Pwd || user.PasswordSalt == password)
            {
                return user;
            }
            return null;
        }

        /// <summary>
        /// 获取一个用户的所有角色
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUserAllRole(int userID)
        {
            RelationUserRoleService relationUserRoleService = new RelationUserRoleService();
            var listRelationUserRole = relationUserRoleService.GetEntities(t => t.UserID == userID);           
            //当前用户使用的角色名称
            return string.Join(",", listRelationUserRole.Select(t => t.Role.Name).ToList());
        }
    }
}
