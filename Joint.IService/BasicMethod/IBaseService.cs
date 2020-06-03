using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Joint.Entity;

namespace Joint.IService
{
    public interface IBaseService<T> where T : BaseModel , new()
    {

        /// <summary>
        /// 判断数据库是否存在满足条件的数据
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<T, bool>> whereLambds);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T AddEntity(T entity);

        /// <summary>
        /// 批量添加多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        List<T> AddEntities(List<T> entities);

        /// <summary>
        /// 实现对数据库的修改功能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool UpdateEntity(T entity);

        /// <summary>
        /// 批量更新多个实体
        /// </summary>
        /// <returns></returns>
        int UpdateEntities(List<T> entitys);

        /// <summary>
        /// 实现对数据库的删除功能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool DeleteEntity(T entity);

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteEntity(int id);

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteEntity(long id);

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteEntity(string id);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(List<string> ids);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(List<int> ids);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(List<long> ids);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(int[] ids);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(long[] ids);

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DeleteEntities(string[] ids);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        int DeleteEntitiesByWhere(Expression<Func<T, bool>> whereLambds);

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        int DeleteEntities(List<T> entitys);

        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda, bool canLazyLoading);

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        T GetEntity(int id, bool canLazyLoading);

        /// <summary>
        /// 通过ID获取实体（默认开启贪婪加载）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntity(int id);

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        T GetEntity(long id, bool canLazyLoading);

        /// <summary>
        /// 通过ID获取实体（默认开启贪婪加载）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntity(long id);

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        T GetEntity(string id, bool canLazyLoading);

        /// <summary>
        /// 通过ID获取实体（默认开启贪婪加载）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntity(string id);

        /// <summary>
        /// 获取前N条记录
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="topNum">获取的条数</param>
        /// <param name="orderByLambda">排序字段</param>
        /// <param name="whereLambda">过滤条件</param>
        /// <param name="isAsc">是否升序排列，默认倒序</param>
        /// <returns></returns>
        IQueryable<T> GetTopEntities<S>(
            int topNum,
            Expression<Func<T, S>> orderByLambda,
            Expression<Func<T, bool>> whereLambda = null,
            bool isAsc = false);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(List<string> ids);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(List<int> ids);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(List<long> ids);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(int[] ids);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(long[] ids);

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetEntities(string[] ids);

        /// <summary>
        /// 实现对数据库的查询  --简单查询
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetEntities(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 获取整个表所有的数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetAllEntities();

        /// <summary>
        /// 实现对数据的分页查询
        /// </summary>
        /// <typeparam name="S">按照某个类进行排序</typeparam>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="pageSize">一页显示多少条数据</param>
        /// <param name="total">总条数</param>
        /// <param name="whereLambda">取得排序的条件</param>
        /// <param name="isAsc">如何排序，根据倒叙还是升序</param>
        /// <param name="orderByLambda">根据那个字段进行排序</param>
        /// <returns></returns>
        IQueryable<T> GetEntitiesByPage<S>(
            int pageIndex,
            int pageSize,
            out int total,
            Expression<Func<T, bool>> whereLambda,
            bool isAsc,
            Expression<Func<T, S>> orderByLambda);

        /// <summary>
        /// 实现对数据的分页查询,返回分页数据模型
        /// </summary>
        /// <typeparam name="S">按照某个类进行排序</typeparam>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="pageSize">一页显示多少条数据</param>
        /// <param name="whereLambda">取得排序的条件</param>
        /// <param name="isAsc">如何排序，根据倒叙还是升序</param>
        /// <param name="orderByLambda">根据那个字段进行排序</param>
        /// <returns></returns>
        PageModel<T> GetEntitiesByPage<S>(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> whereLambda,
            bool isAsc,
            Expression<Func<T, S>> orderByLambda);

    }
}
