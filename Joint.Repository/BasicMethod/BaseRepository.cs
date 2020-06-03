using System.Data;
using Joint.Entity;
using Joint.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using EntityFramework.Extensions;

namespace Joint.Repository
{
    /// <summary>
    /// 实现对数据库的操作(增删改查)的基类
    /// </summary>
    /// <typeparam name="T">定义泛型，约束其是一个类</typeparam>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseModel, new()
    {
        /// <summary>
        /// 创建EF框架的上下文
        /// </summary>
        protected DbContext db = EFContextFactory.GetCurrentDbContext();

        /// <summary>
        /// 判断数据库是否存在满足条件的数据
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<T, bool>> whereLambds)
        {            
            return db.Set<T>().AsNoTracking().Any(whereLambds);
        }

        /// <summary>
        /// 实现对数据库的添加功能,添加实现EF框架的引用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T AddEntity(T entity)
        {
            //EF4.0的写法   添加实体
            //db.CreateObjectSet<T>().AddObject(entity);
            //EF5.0的写法
            db.Entry<T>(entity).State = EntityState.Added;            
            //下面的写法统一
            //db.SaveChanges();            
            return entity;
        }

        /// <summary>
        /// 批量添加多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual List<T> AddEntities(List<T> entities)
        {
            return db.Set<T>().AddRange(entities).ToList();

            //List<T> addEntities = new List<T>();
            //foreach (var item in entities)
            //{
            //    var data = AddEntity(item);
            //    addEntities.Add(data);
            //}
            //return addEntities;
            //return null;
        }

        /// <summary>
        /// 实现对数据库的修改功能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool UpdateEntity(T entity)
        {
            //EF4.0的写法
            //db.CreateObjectSet<T>().Addach(entity);
            //db.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            //EF5.0的写法

            //dynamic tempEntity = entity;
            //int id = tempEntity.ID;
            //if (id != null)
            //{
            //    tempEntity = db.Set<T>().Find(id);
            //    if (tempEntity != null)
            //    {
            //        db.Entry<T>(entity).State = EntityState.Detached; //这个是在同一个上下文能修改的关键
            //    }
            //}

            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Modified;
            //int aaaaaaa = db.SaveChanges();
            //return aaaaaaa > 0;
            return true;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual bool UpdateEntities(List<T> entitys)
        {
            foreach (var item in entitys)
            {
                UpdateEntity(item);
            }
            return true;
        }


        //实现对数据库的删除功能
        public virtual bool DeleteEntity(T entity)
        {
            //EF4.0的写法
            //db.CreateObjectSet<T>().Addach(entity);
            //db.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
            //EF5.0的写法
            //db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Deleted;
            //return db.SaveChanges() > 0;
            return true;
        }


        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(int id)
        {
            T entity = db.Set<T>().Find(id);
            db.Entry<T>(entity).State = EntityState.Deleted;
            //return db.SaveChanges() > 0;
            return true;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(long id)
        {
            T entity = db.Set<T>().Find(id);
            db.Entry<T>(entity).State = EntityState.Deleted;
            //return db.SaveChanges() > 0;
            return true;
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteEntity(string id)
        {
            T entity = db.Set<T>().Find(id);
            db.Entry<T>(entity).State = EntityState.Deleted;
            //return db.SaveChanges() > 0;
            return true;
        }

        /// <summary>
        /// 根据条件批量删除实体对象
        /// </summary>
        /// <param name="whereLambds"></param>
        /// <returns></returns>
        public virtual bool DeleteEntitiesByWhere(Expression<Func<T, bool>> whereLambds)
        {
            if (db.Set<T>().Any(whereLambds))
            {
                return db.Set<T>().Where(whereLambds).Delete() > 0;
            }
            return true;

            //var data = db.Set<T>().Where<T>(whereLambds).ToList();
            //return DeleteEntities(data);
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(List<T> entitys)
        {
            //return db.Set<T>().Where(t => t.ID == 1).Delete() > 0;

            foreach (var item in entitys)
            {
                DeleteEntity(item);
                //db.Set<T>().Attach(item);
                //db.Entry<T>(item).State = EntityState.Deleted;
            }
            //return db.SaveChanges() > 0;
            return true;
        }


        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(List<string> ids)
        {
            foreach (var id in ids)
            {
                DeleteEntity(id);
                //T entity = db.Set<T>().Find(id);
                //if (entity != null)
                //{
                //    db.Entry<T>(entity).State = EntityState.Deleted;
                //}
            }
            //return db.SaveChanges() > 0;
            return true;
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteEntity(id);
            }
            return true;
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(List<long> ids)
        {
            foreach (var id in ids)
            {
                DeleteEntity(id);
            }
            return true;
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(int[] ids)
        {
            return DeleteEntities(ids.ToList());
        }

        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(long[] ids)
        {
            return DeleteEntities(ids.ToList());
        }


        /// <summary>
        /// 根据多个ID删除实体
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteEntities(string[] ids)
        {

            return DeleteEntities(ids.ToList());
        }


        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda)
        {
            IQueryable<T> data = GetEntities(whereLambda);
            return data.FirstOrDefault();
        }


        /// <summary>
        /// 获取满足条件的第一个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="canLazyLoading">是否开启贪婪加载</param>
        /// <returns></returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> whereLambda, bool canLazyLoading)
        {
            IQueryable<T> data = GetEntities(whereLambda);
            if (canLazyLoading)
            {
                return data.FirstOrDefault();
            }
            else
            {
                return data.AsNoTracking().FirstOrDefault();
            }
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(int id, bool canLazyLoading)
        {
            T entity = db.Set<T>().Find(id);
            if (!canLazyLoading)
            {
                if (entity != null)
                {
                    db.Entry<T>(entity).State = EntityState.Detached; //这个是在同一个上下文能修改的关键
                }
            }
            return entity;
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(int id)
        {
            return GetEntity(id, true);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(long id, bool canLazyLoading)
        {
            T entity = db.Set<T>().Find(id);
            if (!canLazyLoading)
            {
                if (entity != null)
                {
                    db.Entry<T>(entity).State = EntityState.Detached; //这个是在同一个上下文能修改的关键
                }
            }

            return entity;
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(long id)
        {
            return GetEntity(id, true);
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(string id, bool canLazyLoading = true)
        {
            T entity = db.Set<T>().Find(id);
            if (!canLazyLoading)
            {
                if (entity != null)
                {
                    db.Entry<T>(entity).State = EntityState.Detached; //这个是在同一个上下文能修改的关键
                }
            }

            return entity;
        }

        /// <summary>
        /// 通过ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetEntity(string id)
        {
            return GetEntity(id, true);
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
        public virtual IQueryable<T> GetTopEntities<S>(int topNum, Expression<Func<T, S>> orderByLambda, Expression<Func<T, bool>> whereLambda = null, bool isAsc = false)
        {
            IQueryable<T> data = db.Set<T>();
            if (isAsc)
            {
                data = db.Set<T>().OrderBy(orderByLambda);
            }
            else
            {
                data = db.Set<T>().OrderByDescending(orderByLambda);
            }

            if (whereLambda != null)
            {
                data = data.Where(whereLambda).Take(topNum);
            }
            else
            {
                data = data.Take(topNum);
            }
            return data;
        }

        /// <summary>
        /// 实现对数据库的查询  --简单查询
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetEntities(Expression<Func<T, bool>> whereLambda)
        {
            //EF4.0的写法
            //return db.CreateObjectSet<T>().Where<T>(whereLambda).AsQueryable();
            //EF5.0的写法
            return db.Set<T>().Where<T>(whereLambda).AsQueryable();
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
            //EF4.0和上面的查询一样
            //EF5.0

            var temp = db.Set<T>().Where<T>(whereLambda);
            total = temp.Count(); //得到总的条数
            //排序,获取当前页的数据
            if (isAsc)
            {
                temp = temp.OrderBy<T, S>(orderByLambda)
                     .Skip<T>(pageSize * (pageIndex - 1)) //越过多少条
                     .Take<T>(pageSize).AsQueryable(); //取出多少条
            }
            else
            {
                temp = temp.OrderByDescending<T, S>(orderByLambda)
                    .Skip<T>(pageSize * (pageIndex - 1)) //越过多少条
                    .Take<T>(pageSize).AsQueryable(); //取出多少条
            }
            return temp.AsQueryable();
        }
    }
}
