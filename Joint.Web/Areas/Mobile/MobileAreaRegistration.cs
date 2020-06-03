using System.Web.Mvc;
using Joint.Web.Framework;

namespace Joint.Web.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistrationOrder
    {
        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {


            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional });

            //context.MapRoute( 
            //    "Mobile_CopyAction",
            //    "Mobile/{controller}/{action}/{id}",
            //    new { controller = "home", action = "Index", id = UrlParameter.Optional });
        }

        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }

        public override int Order
        {
            get
            {
                return 2;
            }
        }
    }
}