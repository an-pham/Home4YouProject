using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class VungController : Controller
    {
        //
     
        HelperController helper = new HelperController();
        string role;
        bool is_login;
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            if (ctx.HttpContext.Session["Role"] != null)
            {
                role = ctx.HttpContext.Session["Role"].ToString();
                is_login = (bool)ctx.HttpContext.Session["IsLogin"];
            }
            //role = ControllerContext.HttpContext.Session["Role"].ToString();
        }
        // GET: /back/Vung/
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }

        
        /****** Action thuộc vùng ******/
        public ActionResult DanhSachVung(string title)
        {
            var dataContext = new HomeForYouDataContext();
            if (title != null)
            {
                List<Vung> vung = dataContext.Vungs.Where(v => v.TenVung.Contains(title) == true).ToList();
                return View("DanhSachVung", vung);
            }
            List<Vung> vungs = dataContext.Vungs.ToList();
            // kiem tra delete, edit
            foreach (var d in vungs)
            {
                if (khong_xoa_sua(d.MaVung))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            return View("DanhSachVung", vungs);
        }

        public ActionResult ThemVung()
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var thanhpho = from c in dataContext.ThanhPhos
                           where c.TinhTrang == "Enabled"
                           select c;
            ViewData["ThanhPho"] = new SelectList(thanhpho, "MaTP", "TenTP");
            return View();
        }
        [HttpPost]
        public ActionResult ThemVung(string tenvung, int thanhpho)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var tp = from c in dataContext.ThanhPhos
                     where c.TinhTrang == "Enabled"
                     select c;
            ViewData["ThanhPho"] = new SelectList(tp, "MaTP", "TenTP");
            Vung vung = new Vung();
            if (tenvung == "")
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                ViewBag.Success = 0;
                var qg = from c in dataContext.QuocGias
                     where c.TinhTrang == "Enabled"
                     select c;
                ViewData["QuocGia"] = new SelectList(qg, "MaQG", "TenQG");
                return View();
            }
            vung.TenVung = tenvung;
            vung.ThanhPho = thanhpho;
            vung.TinhTrang = "Enabled";
            vung.Xoa = true;
            vung.Sua = true;
            dataContext.Vungs.InsertOnSubmit(vung);
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn đã thêm vùng" + tenvung + " thành công";
            ViewBag.Success = 1;
            return View();
        }

        [HttpGet]
        public ActionResult SuaVung(int mavung)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var tp = from c in dataContext.ThanhPhos
                     where c.TinhTrang == "Enabled"
                     select c;
            ViewData["ThanhPho"] = new SelectList(tp, "MaTP", "TenTP");

            var vung = (from s in dataContext.Vungs
                        where s.MaVung == mavung
                        select s).Single();
            return View(vung);
        }

        [HttpPost]
        public ActionResult SuaVung(int mavung, string tenvung, int thanhpho, string tinhtrang)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var tp = from c in dataContext.ThanhPhos
                     where c.TinhTrang == "Enabled"
                     select c;
            ViewData["ThanhPho"] = new SelectList(tp, "MaTP", "TenTP");
            
            var vung = (from s in dataContext.Vungs
                        where s.MaVung == mavung
                        select s).Single();
            if (tenvung == null || tenvung == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View(vung);
            }
            vung.TenVung = tenvung;
            vung.ThanhPho = thanhpho;
            vung.TinhTrang = tinhtrang;

            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn cập nhật vùng " + tenvung + " thành công";
            return View(vung);
        }

        public bool khong_xoa_sua(int ID)
        {
            var dataContext = new HomeForYouDataContext();
            var ct = (from c in dataContext.ChiTietDatDeals
                      where c.Deal.KhachSan1.Vung1.MaVung == ID
                      select c).Count();
            if (ct == 0)
            {
                return false;   //cho xoa
            }
            return true;    //khong cho xoa
        }
        
        public void Xoa(int MaVung)
        {
            var dataContext = new HomeForYouDataContext();
            var vung = (from s in dataContext.Vungs
                        where s.MaVung == MaVung
                        select s).Single();
            vung.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
        }
    }
}
