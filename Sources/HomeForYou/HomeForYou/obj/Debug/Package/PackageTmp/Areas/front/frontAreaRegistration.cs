using System.Web.Mvc;

namespace HomeForYou.Areas.front
{
    public class frontAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "front";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "front_default",
                "front/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
