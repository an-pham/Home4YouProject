using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace HomeForYou.Areas.front.Controllers
{
    public class HelperController : Controller
    {
        //
        // GET: /front/Helper/

        public ActionResult Index()
        {
            return View();
        }
        public bool Valid_Email(string email)
        {
            string mail_regex = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
            if (Regex.IsMatch(email, mail_regex))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public JsonResult Valid_Email_JSON(string email)
        {
            string mail_regex = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
            if (Regex.IsMatch(email, mail_regex))
            {
                return Json(1);
            }
            return Json(0);
        }

    }
}
