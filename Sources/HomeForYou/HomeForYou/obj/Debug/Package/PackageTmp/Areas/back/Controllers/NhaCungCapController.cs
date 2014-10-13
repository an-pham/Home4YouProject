using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class NhaCungCapController : Controller
    {
        //
        // GET: /back/NhaCungCap/
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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult DanhSach(string title)
        {
           //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
			if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            var dsNhaCC = (from ncc in dataContext.NhaCungCaps 
                           where (title!=null && title!="" && ncc.TenNCC.Contains(title))||(title==null||title=="")
                           select ncc);
            // kiem tra delete, edit
            foreach (var d in dsNhaCC)
            {
                if (khong_xoa_sua(d.MaNCC))
                {
                    d.Xoa = false;
                }
            }
            return View(dsNhaCC);
        }
        public ActionResult Sua(int MaNCC)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
			if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            var NCC = (from NhaCungCap in dataContext.NhaCungCaps
                       where NhaCungCap.MaNCC == MaNCC
                       select NhaCungCap).Single();
            return View(NCC);
        }
        [HttpPost]
        public ActionResult Sua(int MaNCC, string TenNCC, string DiaChi, string Email, string SoDT, string Website, string tinhtrang)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            var NCC = (from NhaCungCap in dataContext.NhaCungCaps
                       where NhaCungCap.MaNCC == MaNCC
                       select NhaCungCap).Single();
            if (TenNCC == null || TenNCC == "" || DiaChi == null || DiaChi == "" || Email == null || Email == "")
            {
                ViewBag.Error = "Cập nhật khuyến mãi thất bại!";
                return View(NCC);
            }
            NCC.TenNCC = TenNCC;
            NCC.DiaChi = DiaChi;
            NCC.Email = Email;
            NCC.SoDT = SoDT;
            NCC.Website = Website;
            NCC.TinhTrang = tinhtrang;
            try
            {
                dataContext.SubmitChanges();
                ViewBag.Message = "Cập nhật nhà cung cấp thành công";
            }
            catch
            {
                ViewBag.Error = "Cập nhật khuyến mãi thất bại!";
            }
            return View(NCC);
        }
        public bool khong_xoa_sua(int MaNCC)
        {
            var dataContext = new HomeForYouDataContext();
            int count = (from ks in dataContext.KhachSans
                         join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                         join c in dataContext.ChiTietDatDeals on d.MaDeal equals c.MaDeal
                         where c.TinhTrang == "Unpaid" && ks.NhaCungCap==MaNCC
                         select ks).Count();
            if (count!=0)
                return true;//Có tồn tại => không cho xóa
            return false;//Cho xóa
        }

        public ActionResult Them()
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            return View();
        }
        [HttpPost]
        public ActionResult Them(string TenNCC, string DiaChi, string Email, string SoDT, string Website, string tinhtrang)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            if (TenNCC == null || TenNCC == "" || DiaChi == null || DiaChi == "" || Email == null || Email == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View();
            }
            NhaCungCap NCC = new NhaCungCap();
            NCC.TenNCC = TenNCC;
            NCC.DiaChi = DiaChi;
            NCC.Email = Email;
            NCC.SoDT = SoDT;
            NCC.Website = Website;
            NCC.TinhTrang = tinhtrang;
            NCC.Xoa = true;
            NCC.Sua = true;
            dataContext.NhaCungCaps.InsertOnSubmit(NCC);
            try
            {
                dataContext.SubmitChanges();
                ViewBag.Message = "Thêm nhà cung cấp " + TenNCC +" thành công";
            }
            catch
            {
                ViewBag.Error = "Cập nhật khuyến mãi thất bại!";
            }
            return View();
        }

        public void Xoa(int MaNCC)
        {
            var dataContext = new HomeForYouDataContext();
            var ncc = (from s in dataContext.NhaCungCaps
                    where s.MaNCC == MaNCC
                    select s).Single();
            ncc.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
        }
    }
}
