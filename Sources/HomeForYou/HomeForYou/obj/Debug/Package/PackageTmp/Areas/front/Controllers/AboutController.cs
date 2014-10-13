using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.front.Controllers
{
    public class AboutController : Controller
    {
        //
        // GET: /front/About/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CauHoi()
        {
            var dataContext = new HomeForYouDataContext();
            var cauhois = (from bch in dataContext.BangCauHois
                            select bch);
            return View(cauhois);
        }

    }
}
