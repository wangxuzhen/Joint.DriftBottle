using Joint.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.IService
{
    public partial interface ICommonService
    {
        /// <summary>
        /// 获取当前部门的员工
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        IEnumerable<SelectListItemModel> GetStoreUser(int storeID);

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
        string ThumbnailImage(string oldPath, int storeID, int width, int height, string mode, string folderName);

        string ZoomAndCutImage(string oldPath, int storeID, int width, int height, string folderName);

    }
}