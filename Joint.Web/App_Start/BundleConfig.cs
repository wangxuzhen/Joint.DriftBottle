using System.Web;
using System.Web.Optimization;

namespace Joint.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            //// 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            //BundleTable.EnableOptimizations = false;

            //_Layout页头css
            bundles.Add(new StyleBundle("~/Areas/Admin/Content/assets/css/HeadCss").Include(
                      "~/Areas/Admin/Content/assets/css/bootstrap.min.css",
                      "~/Areas/Admin/Content/assets/css/font-awesome.min.css",
                      "~/Areas/Admin/Content/assets/css/ace.min.css",
                      "~/Areas/Admin/Content/assets/css/ace-rtl.min.css",
                      "~/Areas/Admin/Content/assets/css/ace-skins.min.css"));

            bundles.Add(new StyleBundle("~/Content/HeadCss").Include(
                "~/Content/bootstrapValidator.min.css"));

            bundles.Add(new StyleBundle("~/Areas/Admin/Content/easyui/HeadCss").Include(
                "~/Areas/Admin/Content/easyui/easyui.min.css"));

            bundles.Add(new StyleBundle("~/Areas/Admin/Content/booNavigation-master/css/HeadCss").Include(
                "~/Areas/Admin/Content/booNavigation-master/css/booNavigation.css"));

            bundles.Add(new StyleBundle("~/Areas/Admin/Content/bootstrap-table/HeadCss").Include(
                "~/Areas/Admin/Content/bootstrap-table/bootstrap-table.css"));


            bundles.Add(new ScriptBundle("~/Scripts/HeadScript").Include(
                    "~/Scripts/ValidatorExpand.js",
                    //"~/Scripts/LodopFuncs.js",
                    "~/Scripts/bootstrapValidator.min.js",
                    //"~/Scripts/Ewin.js",
                    "~/Scripts/Common.Core.js"));

            bundles.Add(new ScriptBundle("~/Areas/Admin/Content/FootScript").Include(
                    "~/Areas/Admin/Content/LaxJquery.js"
            ));

            bundles.Add(new ScriptBundle("~/Areas/Admin/Content/booNavigation-master/js/FootScript").Include(
                    "~/Areas/Admin/Content/booNavigation-master/js/booNavigation.js"
            ));

            bundles.Add(new ScriptBundle("~/Areas/Admin/Content/bootstrap-table/FootScript").Include(
                    "~/Areas/Admin/Content/bootstrap-table/bootstrap-table.js"
            ));

            bundles.Add(new ScriptBundle("~/Areas/Admin/Content/bootstrap-table/locale/FootScript").Include(
                    "~/Areas/Admin/Content/bootstrap-table/locale/bootstrap-table-zh-CN.min.js"
            ));
        }
    }
}
