using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Joint.Common
{
    public static class DataCache
    {
        #region 删除缓存
        /// <summary>  
        /// 删除缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        public static void DeleteCache(string CacheKey)
        {
            HttpRuntime.Cache.Remove(CacheKey);
        }
        #endregion

        #region 获取缓存
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <returns></returns>  
        public static object GetCache(string CacheKey)
        {
            return HttpRuntime.Cache[CacheKey];
        }
        #endregion

        #region 简单的插入缓存
        /// <summary>  
        /// 简单的插入缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        public static void SetCache(string CacheKey, object objObject)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject);
        }
        #endregion

        #region 有过期时间的插入缓存数据
        /// <summary>  
        /// 有过期时间的插入缓存数据,绝对过期时间，当超过设定时间，立即移除。 
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        /// <param name="absoluteExpiration">过期时间</param>  
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
        }
        #endregion

        #region 插入缓存数据，指定缓存多少秒
        /// <summary>  
        /// 插入缓存，设置超时时间，当超过设定时间没再使用时，才移除缓存
        /// </summary>  
        /// <param name="CacheKey">缓存的键</param>  
        /// <param name="objObject">缓存的数据</param>  
        /// <param name="seconds">缓存秒数</param>  
        /// <param name="slidingExpiration">传null就是禁用可调度过期</param>  
        public static void SetCache(string CacheKey, object objObject, TimeSpan timeSpan)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, System.Web.Caching.Cache.NoAbsoluteExpiration, timeSpan);
        }
        #endregion

        #region 依赖文件的缓存，文件没改不会过期
        /// <summary>  
        /// 依赖文件的缓存，文件没改不会过期  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        /// <param name="depfilename">依赖文件，可调用 DataCache 里的变量</param>  
        public static void SetCacheDepFile(string CacheKey, object objObject, string depfilename)
        {
            //缓存依赖对象  
            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(depfilename);
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                CacheKey,
                objObject,
                dep,
                System.Web.Caching.Cache.NoAbsoluteExpiration, //从不过期  
                System.Web.Caching.Cache.NoSlidingExpiration, //禁用可调过期  
                System.Web.Caching.CacheItemPriority.Default,
                null);
        }
        #endregion


        //public void ShowCache()
        //{
        //    //IDictionaryEnumerator CacheEnum = HttpRuntime.Cache.GetEnumerator();
        //    var totalCount = HttpRuntime.Cache.Count;
        //    //while (CacheEnum.MoveNext())
        //    //{
        //    //    //CacheEnum.Key
        //    //    //cacheItem = Server.HtmlEncode(CacheEnum.Current.ToString());
        //    //    //Response.Write(cacheItem);
        //    //}
        //}

    }
}
