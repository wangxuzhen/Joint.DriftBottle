using Joint.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
namespace Joint.Repository
{
    public class DbSessionFactory
    {
        //保证了线程内DbSession实例唯一
        public static IDbSession GetCurrentDbSession()
        {
            //这里的GetData()方法的key不能和上下文的一样
            IDbSession _dbSession = CallContext.GetData("DbSession") as IDbSession;
            //IDbSession _dbSession = CallContext.LogicalGetData("DbSession") as IDbSession;
            if (_dbSession == null)
            {
                _dbSession = new DbSession();
                //将值设置到数据槽里面去
                CallContext.SetData("DbSession", _dbSession);
                //CallContext.LogicalSetData("DbSession", _dbSession);
            }
            return _dbSession;
        }

        public static void NewDbContext()
        {
            //先保存之前的
            GetCurrentDbSession().SaveChanges();             
            //开辟一个新的
            Repository.EFContextFactory.NewDbContext();

            IDbSession _dbSession = new DbSession();
            //将值设置到数据槽里面去
            CallContext.SetData("DbSession", _dbSession);
        }
    }
}
