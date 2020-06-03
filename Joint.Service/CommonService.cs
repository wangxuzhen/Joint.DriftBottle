using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IRepository;
using Joint.IService;
using Joint.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Joint.Service
{
    public partial class CommonService : ICommonService    
    {
        public IEnumerable<SelectListItemModel> GetStoreUser(int storeID)
        {
            UsersService usersService = new UsersService();
            var storeAllUser = usersService.GetEntities(t => t.DefaultStoreID == storeID && t.Disabled == false && t.IsIntention != true).ToList().Select(t => new SelectListItemModel
            {
                Value = t.ID.ToString(),
                Text = t.RealName
            }).ToList();
            return storeAllUser;
        }

        /// <summary>
        /// 图片剪裁
        /// </summary>
        /// <param name="oldPath">原图片路径</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="mode">生成缩略图的方式:HW指定高宽缩放(可能变形);W指定宽，高按比例 H指定高，宽按比例 Cut指定高宽裁减(不变形)</param>
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        public string ThumbnailImage(string oldPath, int storeID, int width, int height, string mode, string folderName)
        {
            string thumbnailPath = string.Empty;
            if (!string.IsNullOrEmpty(oldPath))
            {
                //生成缩略图  并删除原图
                string fileFullName = FileHelper.Move(oldPath, "/Upload/Reality/" + storeID + "/" + folderName + "/");
                string extension = System.IO.Path.GetExtension(fileFullName);
                //缩略图路径
                thumbnailPath = ImgHelper.GetThumbnailPathByWidth(fileFullName, 60);
                //生成缩略图
                ImgHelper.MakeThumbnail(
                    System.Web.HttpContext.Current.Server.MapPath(fileFullName),
                    System.Web.HttpContext.Current.Server.MapPath(thumbnailPath),
                    width,
                    height,
                    mode,
                    extension
                 );
                FileHelper.DeleteFile(System.Web.HttpContext.Current.Server.MapPath(fileFullName));
            }
            return thumbnailPath;
        }
        public string ZoomAndCutImage(string oldPath, int storeID, int width, int height, string folderName)
        {
            string saveUrlPath = string.Empty;
            if (!string.IsNullOrEmpty(oldPath))
            {
                //生成缩略图  并删除原图
                string fileFullName = FileHelper.Move(oldPath, "/Upload/Reality/" + storeID + "/" + folderName + "/");
                string extension = System.IO.Path.GetExtension(fileFullName);
                //缩略图路径
                saveUrlPath = ImgHelper.GetThumbnailPathByWidth(fileFullName, 60);
                //生成缩略图
                ImgHelper.ZoomAndCutImage(
                    System.Web.HttpContext.Current.Server.MapPath(fileFullName),
                    System.Web.HttpContext.Current.Server.MapPath(saveUrlPath),
                    width,
                    height,
                    12
                 );
                FileHelper.DeleteFile(System.Web.HttpContext.Current.Server.MapPath(fileFullName));
            }
            return saveUrlPath;
        }

    }
}
