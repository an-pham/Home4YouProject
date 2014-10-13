using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class ThanhPhoController : Controller
    {
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
        // GET: /back/ThanhPho/
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }
        /******* Action thuộc thành phố *****/
        public ActionResult DanhSachThanhPho(string title)
        {
            var dataContext = new HomeForYouDataContext();
            if (title != null)
            {
                List<ThanhPho> thanhpho = dataContext.ThanhPhos.Where(tpho => tpho.TenTP.Contains(title) == true).ToList();
                return View("DanhSachThanhPho", thanhpho);
            }
            List<ThanhPho> tp = dataContext.ThanhPhos.ToList();
            // kiem tra delete, edit
            foreach (var d in tp)
            {
                if (khong_xoa_sua(d.MaTP))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            return View("DanhSachThanhPho", tp);
        }
        
        public ActionResult ThemThanhPho()
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var quocgia = from c in dataContext.QuocGias
                          where c.TinhTrang == "Enabled"
                          select c;
            ViewData["QuocGia"] = new SelectList(quocgia, "MaQG", "TenQG");
            return View();
        }
        [HttpPost]
        public ActionResult ThemThanhPho(string tenthanhpho, int quocgia)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var qg = from c in dataContext.QuocGias
                     where c.TinhTrang == "Enabled"
                     select c;
            ViewData["QuocGia"] = new SelectList(qg, "MaQG", "TenQG");
            if (tenthanhpho == "" || quocgia == 0)
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                ViewBag.Success = 0;
                return View();
            }
            
            ThanhPho thanhpho = new ThanhPho();
            thanhpho.TenTP = tenthanhpho;
            thanhpho.QuocGia = quocgia;
            thanhpho.TinhTrang = "Enabled";
            thanhpho.Sua = true;
            thanhpho.Xoa = true;
            dataContext.ThanhPhos.InsertOnSubmit(thanhpho);
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn đã thêm thành phố " + tenthanhpho + " thành công";
            ViewBag.Success = 1;
            return View();
        }

        [HttpGet]
        public ActionResult SuaThanhPho(int matp)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var qg = from p in dataContext.QuocGias
                     where p.TinhTrang == "Enabled"
                     select p;
            ViewData["QuocGia"] = new SelectList(qg, "MaQG", "TenQG");


            var thanhpho = (from s in dataContext.ThanhPhos
                            where s.MaTP == matp
                            select s).Single();
            return View(thanhpho);
        }

        [HttpPost]
        public ActionResult SuaThanhPho(int matp, string tenthanhpho, int quocgia, string tinhtrang)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var qg = from c in dataContext.QuocGias
                     where c.TinhTrang == "Enabled"
                     select c;
            ViewData["QuocGia"] = new SelectList(qg, "MaQG", "TenQG");
            var thanhpho = (from s in dataContext.ThanhPhos
                            where s.MaTP == matp
                            select s).Single();
            if (tenthanhpho == null || tenthanhpho == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View(thanhpho);
            }
            thanhpho.TenTP = tenthanhpho;
            thanhpho.QuocGia = quocgia;
            thanhpho.TinhTrang = tinhtrang;
            
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn cập nhật thành phố " + tenthanhpho + " thành công";
            return View(thanhpho);
        }

        public bool khong_xoa_sua(int ID)
        {
            var dataContext = new HomeForYouDataContext();
            var ct = (from c in dataContext.ChiTietDatDeals
                      where c.Deal.KhachSan1.Vung1.ThanhPho1.MaTP == ID
                      select c).Count();
            if (ct == 0)
            {
                return false; //cho xoa
            }
            return true;    //khong cho xoa
        }
       
        public void XoaThanhPho(int MaTP)
        {
            var dataContext = new HomeForYouDataContext();
            var thanhpho = (from s in dataContext.ThanhPhos
                            where s.MaTP == MaTP
                            select s).Single();
            thanhpho.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
        }

    }
}
