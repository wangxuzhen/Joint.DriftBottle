using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Joint.IRepository
{
    public partial interface IDbSession
    {
        //每个表对应的实体仓储对象
        //IDAL.IRoleRepository RoleRepository { get; }
        //IDAL.IUserInfoRepository UserInfoRepository { get; }
        //将当前应用程序跟数据库的会话内所有实体的变化更新会数据库
        int SaveChanges();

        /// <summary>
        /// 自动更新开关
        /// </summary>
        /// <returns></returns>
        void CanSaveChanges(bool autoSave);

        /// <summary>
        /// 打开关闭贪婪加载
        /// </summary>
        /// <param name="enabled"></param>
        void LazyLoadingEnabled(bool enabled);

        /// <summary>
        /// 执行Sql脚本的方法返回 DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>

        DataSet SqlQueryForDataSet(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// EF直行SQL分页，返回一个DataSet
        /// 注意：parameters需要传入pageSize页面条数，pageIndex第几页
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataSet SqlQueryForDataSetByPage(string sql, out int total, params SqlParameter[] parameters);

        DataSet SqlQueryForDataSetByPage2(string sql, out int total, string orderBy, params SqlParameter[] parameters);

        DataSet SqlQueryForDataSetByPageByWhere(string sql, out int total, string sqlWhere, params SqlParameter[] parameters);
        DataSet SqlQueryForDataSetByPageByWhere(string sql, out int total, string orderBy, string sqlWhere, params SqlParameter[] parameters);



        /// <summary>
        /// EF直行SQL分页，返回一个DataSet
        /// 注意：parameters需要传入pageSize页面条数，pageIndex第几页
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataSet SqlQueryForDataSetByPage(string sql, out int total, string orderBy, params SqlParameter[] parameters);

        /// <summary>
        /// 批量更新数据库数据（内部使用，用户传入的参数不要用此方法，会有注入漏洞）
        /// </summary>
        /// <param name="sql">要批量执行的sql语句</param>
        /// <param name="needTran">是否开启事务，默认开启true</param>
        void BatchUpdate(string sql, bool needTran = true);

        /// <summary>
        /// 执行Sql 返回List 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> SqlQuery<T>(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// 执行Sql 返回受影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql, params SqlParameter[] parameters);
    }
}
