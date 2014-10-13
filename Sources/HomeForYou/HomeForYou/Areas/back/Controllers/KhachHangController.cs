using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class KhachHangController : Controller
    {
        //
        // GET: /back/KhachHang/
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
        [HttpGet]
        public void Index()
        {
            Response.Redirect("List");
        }
        public ActionResult DanhSach(string title)
        {
            // manage role ko quan tam
            /*    if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                    return RedirectToAction("Login", "Admin");*/
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam

            var dataContext = new HomeForYouDataContext();
            
            string base_URL = "danhsach"; //quan trong, ten action cua controller

            //phan trang ko quan tam
            int total_rows = (from a in dataContext.KhachHangs
                              select a).Count();
            if (title != null) // dung cho viec search
            {
                total_rows = (from a in dataContext.KhachHangs
                              where a.Ten.Contains(title)
                              select a).Count();
            }
            ViewBag.total_rows = total_rows;
            string URL_segment;
            try
            {
                URL_segment = Request.Url.Segments[4];
            }
            catch (Exception)
            {
                URL_segment = null;
            }
            Libraries.Pagination pagination = new Libraries.Pagination(base_URL, URL_segment, total_rows);
            string pageLinks = pagination.GetPageLinks();
            int start = (pagination.CurPage - 1) * pagination.PerPage;
            int offset = pagination.PerPage;
            if (URL_segment != null)
                pageLinks = pageLinks.Replace(base_URL + "/", "");
            ViewBag.pageLinks = pageLinks;
            //phan trang ko quan tam

            var KhachHangs = (from a in dataContext.KhachHangs
                             select a).Skip(start).Take(offset);
            if (title != null) // dung cho viec search
            {
                KhachHangs = (from a in dataContext.KhachHangs
                             where a.Ten.Contains(title)
                             select a).Skip(start).Take(offset);
            }

            // kiem tra delete, edit
            foreach (var d in KhachHangs)
            {
                if (kietra_xoa_sua(d))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            return View(KhachHangs);
        }

        [HttpGet]
        public ActionResult Sua(int ID)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var khachhang = (from s in dataContext.KhachHangs
                            where s.ID == ID
                            select s).Single();
            return View(khachhang);
        }
        [HttpPost]
        public ActionResult Sua(int ID, string ho, string ten, string diachi, string cmnd, string sodt, string email, string tinhtrang)
        {
            // manage role ko quan tam
            if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                return RedirectToAction("Login", "Admin");
            helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role);
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var khachhang = (from s in dataContext.KhachHangs
                            where s.ID == ID
                            select s).Single();
            if (ten=="" || diachi=="" || cmnd==""||email=="")
            {
                ViewBag.Message = "Không để trống các thông tin cần thiết.";
                ViewBag.Success = 0;
                return View(khachhang);
            }
            khachhang.Ho = ho;
            khachhang.Ten = ten;
            khachhang.DiaChi = diachi;
            khachhang.CMND = cmnd;
            khachhang.SoDT = sodt;
            khachhang.Email = email;
            khachhang.TinhTrang = tinhtrang;
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn cập nhật khách hàng " + ho + ten + " thành công";
            ViewBag.Success = 1;
            return View(khachhang);
        }
        public bool kietra_xoa_sua(KhachHang kh)
        {
            foreach (var c in kh.ChiTietDatDeals)
            {
                if (c.TinhTrang == "Unpaid")
                {
                    return true;
                }
            }
            return false;
        }
      
        public void Xoa(int ID)
        {
            var dataContext = new HomeForYouDataContext();
            var khachhang = (from s in dataContext.KhachHangs
                            where s.ID == ID
                            select s).Single();
                khachhang.TinhTrang = "Deleted";
                dataContext.SubmitChanges();
            //khachhang.TinhTrang = "Deleted";
            //dataContext.SubmitChanges();
        }
        [HttpGet]
        public ActionResult LichSu(int ID, int? tt=null, int? huy=null)
        {
            // manage role
            if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                return RedirectToAction("Login", "Admin");
            helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role);
            // manage role 
            var dataContext = new HomeForYouDataContext();

            var khachhang = (from s in dataContext.KhachHangs
                             where s.ID == ID && s.TinhTrang=="Enabled"
                             select s).Single();
            var chitiet = (from s in dataContext.KhachHangs
                           join ct in dataContext.ChiTietDatDeals on s.ID equals ct.MaKH
                           where s.ID == ID
                           select ct);
            ViewBag.ChiTiet = chitiet;
            if (huy == 1)
            {
                ViewBag.Message = "Hủy đặt deal mã số " + ID.ToString() + " thành công";
            }
            if (tt == 1)
            {
                ViewBag.Message = "Thanh toán đặt deal mã số " + ID.ToString() + " thành công";
            }
            return View(khachhang);
        }

        public void Huy(int MaCT, int MaKH)
        {
            var dataContext = new HomeForYouDataContext();
            var chitiet = (from s in dataContext.ChiTietDatDeals
                             where s.MaCT == MaCT
                             select s).Single();
            chitiet.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
            //var t = "LichSu/" + MaKH.ToString();
            var t = "LichSu/" + MaKH.ToString() + "?huy=1";
            Response.Redirect(t);
        }
        public void ThanhToan(int MaCT, int MaKH)
        {
            var dataContext = new HomeForYouDataContext();
            var chitiet = (from s in dataContext.ChiTietDatDeals
                           where s.MaCT == MaCT
                           select s).Single();
            var kh = (from s in dataContext.KhachHangs
                           where s.ID == MaKH
                           select s).Single();
            chitiet.TinhTrang = "Paid";
            kh.TichLuy++;
            dataContext.SubmitChanges();
            var t = "LichSu/" + MaKH.ToString() + "?tt=1"; ;
            Response.Redirect(t);
        }
    }
}
