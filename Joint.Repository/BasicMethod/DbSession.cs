using Joint.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Repository
{
    //一次跟数据库交互的会话
    public class DbSession : IDbSession //代表应用程序跟数据库之间的一次会话，也是数据库访问层的统一入口
    {
        public bool canSaveChanges = true;

        //代表：当前应用程序跟数据库的绘画内所有的实体的变化，更新会数据库
        public int SaveChanges()
        {
            if (canSaveChanges)
            {
                //调用EF上下文的SaveChanges方法
                return Repository.EFContextFactory.GetCurrentDbContext().SaveChanges();
            }
            return 0;
        }

        public void CanSaveChanges(bool canSaveChanges)
        {
            this.canSaveChanges = canSaveChanges;
        }


        /// <summary>
        /// 打开关闭贪婪加载
        /// </summary>
        /// <param name="enabled"></param>
        public void LazyLoadingEnabled(bool enabled)
        {
            Repository.EFContextFactory.GetCurrentDbContext().Configuration.ProxyCreationEnabled = enabled;
            Repository.EFContextFactory.GetCurrentDbContext().Configuration.LazyLoadingEnabled = enabled;
        }

        /// <summary>
        /// EF SQL 语句返回 DataSet
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet SqlQueryForDataSet(string sql, params SqlParameter[] parameters)
        {
            return DbHelperSQL.Query(sql, parameters);
        }

        /// <summary>
        /// EF直行SQL分页，返回一个DataSet
        /// 注意：parameters需要传入pageSize页面条数，pageIndex第几页
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet SqlQueryForDataSetByPage(string sql, out int total, params SqlParameter[] parameters)
        {
            //string strSql = string.Format(
            //       "SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY ID DESC) AS IDRank FROM ({0}) K) AS IDWithRowNumber WHERE  IDRank >@pageSize * (@pageIndex-1) AND IDRank <= @pageSize * @pageIndex ",
            //        sql);
            //string strSqlTotal = string.Format("SELECT COUNT(ID) FROM ({0}) K", sql);

            //total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);

            //return DbHelperSQL.Query(strSql, parameters);
            return SqlQueryForDataSetByPage(sql, out total, "ID DESC", parameters);
        }



        public DataSet SqlQueryForDataSetByPage(string sql, out int total, string orderBy, params SqlParameter[] parameters)
        {
            //如果排序条件为空，默认以ID排序
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "ID DESC";
            }

            string strSql = string.Format(
                   "SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY {0}) AS IDRank FROM ({1}) K) AS IDWithRowNumber WHERE  IDRank >@pageSize * (@pageIndex-1) AND IDRank <= @pageSize * @pageIndex ",
                    orderBy, sql);
            string strSqlTotal = string.Format("SELECT COUNT(1) FROM ({0}) K", sql);

            //total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);

            //return DbHelperSQL.Query(strSql, parameters);

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.AppendLine(strSql);
            sbSQL.AppendLine(strSqlTotal);
            var dsList = DbHelperSQL.Query(sbSQL.ToString(), parameters);
            total = Convert.ToInt32(dsList.Tables[1].Rows[0][0]);
            return dsList;
        }


        public DataSet SqlQueryForDataSetByPage2(string sql, out int total, string orderBy, params SqlParameter[] parameters)
        {
            //如果排序条件为空，默认以ID排序

            string strSql = string.Format(
                   "SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY {0}) AS IDRank FROM ({1}) K) AS IDWithRowNumber WHERE  IDRank >@pageSize * (@pageIndex-1) AND IDRank <= @pageSize * @pageIndex ",
                    orderBy, sql);
            string strSqlTotal = string.Format("SELECT COUNT(1) FROM ({0}) K", sql);

            total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);

            return DbHelperSQL.Query(strSql, parameters);

            //StringBuilder sbSQL = new StringBuilder();
            //sbSQL.AppendLine(strSql);
            ////sbSQL.AppendLine("GO");
            //sbSQL.AppendLine(strSqlTotal);
            //var dsList = DbHelperSQL.Query(sbSQL.ToString(), parameters);
            //total = Convert.ToInt32(dsList.Tables[1].Rows[0][0]);
            //return dsList;
        }

        public DataSet SqlQueryForDataSetByPageByWhere(string sql, out int total, string sqlWhere, params SqlParameter[] parameters)
        {
            //string strSql = string.Format(
            //       "SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY ID DESC) AS IDRank FROM ({0}) K) AS IDWithRowNumber WHERE  IDRank >@pageSize * (@pageIndex-1) AND IDRank <= @pageSize * @pageIndex ",
            //        sql);
            //string strSqlTotal = string.Format("SELECT COUNT(ID) FROM ({0}) K", sql);

            //total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);

            //return DbHelperSQL.Query(strSql, parameters);
            return SqlQueryForDataSetByPageByWhere(sql, out total, "ID DESC", sqlWhere, parameters);
        }
        public DataSet SqlQueryForDataSetByPageByWhere(string sql, out int total, string orderBy, string sqlWhere, params SqlParameter[] parameters)
        {
            //如果排序条件为空，默认以ID排序
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "ID DESC";
            }

            string strSql = string.Format(
                   "SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY {0}) AS IDRank FROM ({1}) K " + sqlWhere + ") AS IDWithRowNumber WHERE  IDRank >@pageSize * (@pageIndex-1) AND IDRank <= @pageSize * @pageIndex ",
                    orderBy, sql);


            total = 0;
            string strSqlTotal = string.Format("SELECT Count(1) FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY {0}) AS IDRank FROM ({1}) K " + sqlWhere + ") AS IDWithRowNumber", orderBy, sql);
            total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);



            //total = Convert.ToInt32(DbHelperSQL.Query(strSqlTotal, parameters).Tables[0].Rows[0][0]);

            return DbHelperSQL.Query(strSql, parameters);
        }

        /// <summary>
        /// 批量更新数据库数据（内部使用，用户传入的参数不要用此方法，会有注入漏洞）
        /// </summary>
        /// <param name="sql">要批量执行的sql语句</param>
        /// <param name="needTran">是否开启事务，默认开启true</param>
        public void BatchUpdate(string sql, bool needTran = true)
        {
            string endSql = @"  BEGIN TRANSACTION
                              --开始事务
                              DECLARE @error INT
                              --定义变量，累积事务执行过程中的错误
                              SET @error = 0
    
                              ---- 执行语句
                              {0}

                              SET @error = @error + @@error
                              ------
                              --判断
                              IF @error <> 0  --有误
                                BEGIN
                                PRINT '1'
                                    ROLLBACK  TRANSACTION
                                END
                              ELSE
                                BEGIN
                                PRINT '2'
                                    COMMIT TRANSACTION
                                END";

            if (needTran)
            {
                endSql = string.Format(endSql, sql);
            }
            else
            {
                endSql = sql;
            }

            DbHelperSQL.ExecuteSql(endSql);
        }

        //执行Sql脚本的方法
        public List<T> SqlQuery<T>(string sql, params SqlParameter[] parameters)
        {
            //Ef4.0的执行方法 ObjectContext
            //封装一个执行SQl脚本的代码
            return Repository.EFContextFactory.GetCurrentDbContext().Database.SqlQuery<T>(sql, parameters).AsEnumerable().ToList();
        }


        //执行Sql脚本的方法
        public int ExecuteSqlCommand(string sql, params SqlParameter[] parameters)
        {
            //Ef4.0的执行方法 ObjectContext
            //封装一个执行SQl脚本的代码
            return Repository.EFContextFactory.GetCurrentDbContext().Database.ExecuteSqlCommand(sql, parameters);
        }

    }
}
