using System.Web.Mvc;
using Joint.Web.Framework;

namespace Joint.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistrationOrder
    {
        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional });

            //context.MapRoute( 
            //    "Admin_CopyAction",
            //    "admin/{controller}/{action}/{id}",
            //    new { controller = "home", action = "Index", id = UrlParameter.Optional });
        }

        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override int Order
        {
            get
            {
                return 0;
            }
        }
    }
}