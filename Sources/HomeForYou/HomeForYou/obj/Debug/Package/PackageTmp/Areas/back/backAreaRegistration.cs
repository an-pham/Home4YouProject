using System.Web.Mvc;

namespace HomeForYou.Areas.back
{
    public class backAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "back";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "back_default",
                "back/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
