using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Joint.Common
{
    /// <summary>
    /// 同一个事务实例，只能被同一个线程使用，其他的使用者等待
    /// </summary>
   public class EFMonitorTransaction : IDisposable
   {
       private readonly static object _MyLock = new object();
       /// <summary>  
       /// 当前事务对象  
       /// </summary>  
       private TransactionScope tran = null;
       public EFMonitorTransaction()
       {
           Monitor.Enter(_MyLock);//获取排它锁  
           this.tran = new TransactionScope();
       }
       /// <summary>  
       /// 提交  
       /// </summary>  
       public void Complete()
       {
           tran.Complete();
       }
       /// <summary>  
       /// 混滚操作,在Dispose(),中自动调用回滚  
       /// </summary>  
       public void Rollback()
       {
           //提前执行释放，回滚  
           if (tran != null)
               tran.Dispose();
       }
       public void Dispose()
       {
           if (tran != null)
               tran.Dispose();
           Monitor.Exit(_MyLock);//释放排它锁  
       }
   }  
}
