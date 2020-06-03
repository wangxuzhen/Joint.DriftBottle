using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Joint.IRepository;
using Joint.IService;
using Joint.Entity;
using System.Linq.Expressions;
using Joint.Repository;

namespace Joint.Service
{
    public abstract class BaseService<T> : IBaseService<T> where T : BaseModel, new()
    {
        public IDbSession dbSession = DbSessionFactory.GetCurrentDbSession();


        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> currentRepository { get; set; }

        /// <summary>
        /// 基类的构造函数
        /// </summary>
        public BaseService(IBaseRepository<T> currentRepository)
        {
            this.currentRepository = currentRepository;
        }

        /// <summary>
        /// 判断数据库是否存在满足条件的数据
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<T, bool>> whereLambds)
        {
            return currentRepository.Exists(whereLambds);
        }

        /// <summary>
        /// 实现对数据库的添加功能,添加实现EF框架的引用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T AddEntity(T entity)
        {
            var addEntity = currentRepository.AddEntity(entity);
            int result = 0;
            try
            {
                //int bbbbbbb = currentRepository.GetHashCode();
                //int aaaaaaaa = dbSession.GetHashCode();
                result = dbSession.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string errmsg = "";
                foreach (var item in ex.EntityValidationErrors.First().ValidationErrors)
                {
                    errmsg += item.ErrorMessage + " ; ";
                }
                throw new Exception(errmsg);
            }

            return addEntity;
        }

        /// <summary>
        /// 批量添加多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual List<T> AddEntities(List<T> entities)
        {
            var addEntities = currentRepository.AddEntities(entities);
            dbSession.SaveChanges();
            return addEntities;
        }


        /// <summary>
        /// 实现对数据库的修改功能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool UpdateEntity(T entity)
        {
            try
            {
                currentRepository.UpdateEntity(entity);
                int result = dbSession.SaveChanges();
                return result > 0;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string errmsg = "";
                foreach (var item in ex.EntityValidationErrors.First().ValidationErrors)
                {
                    errmsg += item.ErrorMessage + " ; ";
                }
                throw new Exception(errmsg);
            }
        }

        /// <summary>
        /// 批量更新多个实体
        /// </summary>
        /// <returns></returns>
        public virtual int UpdateEntities(List<T> entitys)
        {
            currentRepository.UpdateEntities(entitys);
            return dbSession.SaveChanges();
        }

        //实现对数据库的删除功能
        public virtual bool DeleteEntity(T entity)
        {
            currentRepository.DeleteEntity(entity);
            return dbSession.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(int id)
        {
            currentRepository.DeleteEntity(id);
            return dbSession.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(long id)
        {
            currentRepository.DeleteEntity(id);
            return dbSession.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(string id)
        {
            currentRepository.DeleteEntity(id);
            return dbSession.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据条件批量删除实体对象
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        public virtual int DeleteEntitiesByWhere(Expression<Func<T, bool>> whereLambds)
        {
            currentRepository.DeleteEntitiesByWhere(whereLambds);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(List<T> entitys)
        {
            currentRepository.DeleteEntities(entitys);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(List<string> ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(List<int> ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(List<long> ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(int[] ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(long[] ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int DeleteEntities(string[] ids)
        {
            currentRepository.DeleteEntities(ids);
            return dbSession.SaveChanges();
        }


        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda)
        {
            return currentRepository.GetFirstOrDefault(whereLambda);
        }

        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda, bool canLazyLoading)
        {
            return currentRepository.GetFirstOrDefault(whereLambda, canLazyLoading);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(int id, bool canLazyLoading)
        {
            return currentRepository.GetEntity(id, canLazyLoading);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(int id)
        {
            return currentRepository.GetEntity(id);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(long id, bool canLazyLoading)
        {
            return currentRepository.GetEntity(id, canLazyLoading);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(long id)
        {
            return currentRepository.GetEntity(id);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(string id, bool canLazyLoading)
        {
            return currentRepository.GetEntity(id, canLazyLoading);
        }


        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(string id)
        {
            return currentRepository.GetEntity(id);
        }

        /// <summary>
        /// 获取前N条记录
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="topNum">获取的条数</param>
        /// <param name="orderByLambda">排序字段</param>
        /// <param name="whereLambda">过滤条件</param>
        /// <param name="isAsc">是否升序排列，默认倒序</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetTopEntities<S>(
              int topNum,
              Expression<Func<T, S>> orderByLambda,
              Expression<Func<T, bool>> whereLambda = null,
              bool isAsc = false)
        {
            return currentRepository.GetTopEntities(
                topNum,
                orderByLambda,
                whereLambda,
                isAsc);
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(List<string> ids)
        {
            List<T> list = new List<T>();
            foreach (var id in ids)
            {
                list.Add(GetEntity(id));
            }
            //return db.SaveChanges() > 0;
            return list;
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(List<int> ids)
        {
            List<T> list = new List<T>();
            foreach (var id in ids)
            {
                list.Add(GetEntity(id));
            }
            return list;
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(List<long> ids)
        {
            List<T> list = new List<T>();
            foreach (var id in ids)
            {
                list.Add(GetEntity(id));
            }
            return list;
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(int[] ids)
        {
            return GetEntities(ids.ToList());
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(long[] ids)
        {
            return GetEntities(ids.ToList());
        }

        /// <summary>
        /// 根据多个ID获取实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetEntities(string[] ids)
        {

            return GetEntities(ids.ToList());
        }

        /// <summary>
        /// 实现对数据库的查询  --简单查询
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetEntities(Expression<Func<T, bool>> whereLambda)
        {
            return currentRepository.GetEntities(whereLambda);
        }

        /// <summary>
        /// 获取整个表所有的数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllEntities()
        {
            return GetEntities(t => true);
        }

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
        public virtual IQueryable<T> GetEntitiesByPage<S>(
            int pageIndex,
            int pageSize,
            out int total,
            Expression<Func<T, bool>> whereLambda,
            bool isAsc,
            Expression<Func<T, S>> orderByLambda)
        {
            //如果whereLambda条件为空，则默认查询所有
            Expression<Func<T, bool>> where;
            if (whereLambda == null)
            {
                where = t => true;
            }
            else
            {
                where = whereLambda;
            }

            return currentRepository.GetEntitiesByPage(
                pageIndex,
                pageSize,
                out total,
                where,
                isAsc,
                orderByLambda);
        }

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
        public virtual PageModel<T> GetEntitiesByPage<S>(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> whereLambda,
            bool isAsc,
            Expression<Func<T, S>> orderByLambda)
        {
            Expression<Func<T, bool>> where;
            if (whereLambda == null)
            {
                where = t => true;
            }
            else
            {
                where = whereLambda;
            }

            int total;
            var data = currentRepository.GetEntitiesByPage(
               pageIndex,
               pageSize,
               out total,
               where,
               isAsc,
               orderByLambda);

            return new PageModel<T>()
            {
                Models = data.ToList(),
                pagingInfo = new PagingInfo
                {
                    Total = total,
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
        }
    }
}
