using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.front.Controllers
{
    public class KhachSanController : Controller
    {
        //
        // GET: /front/KhachSan/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Rating(int ma_ks, int vote)
        {
            var dataContext = new HomeForYouDataContext();
            var hotel = (from h in dataContext.KhachSans
                         where h.MaKS == ma_ks
                         select h).Single();
            float max_point = (from h in dataContext.KhachSans
                               select (float)h.Diem).Max();
            float min_point = (from h in dataContext.KhachSans
                               select (float)h.Diem).Min();
            float diem_cong = (float)hotel.DiemCong;
            float diem_tru = (float)hotel.DiemTru;
            if(vote == 1 )
            {
                diem_cong = (float)hotel.DiemCong + vote;
            }
            else
            {
                diem_tru = (float)hotel.DiemTru + Math.Abs(vote);
            }
            hotel.Diem = Math.Round((double)((diem_cong - diem_tru) * 9 / 1000 + 1),2);
            hotel.DiemCong =(int)diem_cong;
            hotel.DiemTru = (int)diem_tru;
            dataContext.SubmitChanges();
            return Json(hotel.Diem);
        }

    }
}
