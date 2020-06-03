using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Joint.Common
{
    public class TransactionScopeHelper
    {
        public static TransactionScope GetTran(IsolationLevel defaultLevel = System.Transactions.IsolationLevel.RepeatableRead)
        {
            TransactionOptions transactionOption = new TransactionOptions();
            //设置事务隔离级别
            transactionOption.IsolationLevel = defaultLevel;
            // 设置事务超时时间为60秒
            transactionOption.Timeout = new TimeSpan(0, 0, 40);
            TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption);
            return scope;
        }

        public static TransactionScope GetTran(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, IsolationLevel defaultLevel = System.Transactions.IsolationLevel.RepeatableRead)
        {
            TransactionOptions transactionOption = new TransactionOptions();
            //设置事务隔离级别
            transactionOption.IsolationLevel = defaultLevel;
            transactionOption.Timeout = scopeTimeout;

            TransactionScope scope = new TransactionScope(scopeOption, transactionOption);

            return scope;
        }
    }
}
