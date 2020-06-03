using System.Web.Mvc;
using Joint.Web.Framework;

namespace Joint.Web.Areas.DriftBottle
{
    public class DriftBottleAreaRegistration : AreaRegistrationOrder
    {
        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DriftBottle_default",
                "DriftBottle/{controller}/{action}/{id}",
                new { controller = "home", action = "Index", id = UrlParameter.Optional });

            //context.MapRoute( 
            //    "DriftBottle_CopyAction",
            //    "DriftBottle/{controller}/{action}/{id}",
            //    new { controller = "home", action = "Index", id = UrlParameter.Optional });
        }

        public override string AreaName
        {
            get
            {
                return "DriftBottle";
            }
        }

        public override int Order
        {
            get
            {
                return 3;
            }
        }
    }
}