using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class NhanVienController : Controller
    {
        //
        // GET: /back/nhanvien/
        HelperController helper = new HelperController();
        string role;
        bool is_login;
        string username;
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            if (ctx.HttpContext.Session["Role"] != null)
            {
                role = ctx.HttpContext.Session["Role"].ToString();
                is_login = (bool)ctx.HttpContext.Session["IsLogin"];
                username = ctx.HttpContext.Session["Username"].ToString();
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
            int total_rows = (from a in dataContext.NhanViens
                              select a).Count();
            if (title != null) // dung cho viec search
            {
                total_rows = (from a in dataContext.NhanViens
                              where a.Username.Contains(title)
                              select a).Count();
            }
            ViewBag.total_rows = total_rows;
            string base_URL = "danhsach"; //quan trong, ten action cua controller

            //phan trang ko quan tam
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

            var NhanViens = (from a in dataContext.NhanViens
                             select a).Skip(start).Take(offset);
            if (title != null) // dung cho viec search
            {
                NhanViens = (from a in dataContext.NhanViens
                             where a.Username.Contains(title)
                             select a).Skip(start).Take(offset);
            }
            return View(NhanViens);
        }

        [HttpGet]
        public ActionResult Them()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Them(string username, string password, string hoten, string cmnd, string chucvu, float luong, string sodt)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            string encrypted_password = helper.Encrypted_Password(password);
            NhanVien nhanvien = new NhanVien();
            nhanvien.Username = username;
            nhanvien.Password = encrypted_password;
            nhanvien.HoTenNV = hoten;
            nhanvien.CMND = cmnd;
            nhanvien.ChucVu = chucvu;
            nhanvien.LuongCB = luong;
            nhanvien.SoDT = sodt;
            nhanvien.TinhTrang = "Enabled";
            dataContext.NhanViens.InsertOnSubmit(nhanvien);
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn đã thêm nhân viên " + hoten + " thành công";
            return View();
        }

        [HttpGet]
        public ActionResult Sua(int MaNV)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var nhanvien = (from s in dataContext.NhanViens
                            where s.MaNV == MaNV
                            select s).Single();
            return View(nhanvien);
        }
        [HttpPost]
        public ActionResult Sua(int MaNV, string hoten, string cmnd, string chucvu, string tinhtrang)
        {
            // manage role ko quan tam
            if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                return RedirectToAction("Login", "Admin");
            helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role);
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var nhanvien = (from s in dataContext.NhanViens
                            where s.MaNV == MaNV
                            select s).Single();
            if (hoten == null || hoten == "" || cmnd == null || cmnd == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View(nhanvien);
            }
            nhanvien.HoTenNV = hoten;
            nhanvien.CMND = cmnd;
            nhanvien.ChucVu = chucvu;
            nhanvien.TinhTrang = tinhtrang;
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn cập nhật nhân viên " + hoten + " thành công";
            return View(nhanvien);
        }
        public void Xoa(int MaNV)
        {
            var dataContext = new HomeForYouDataContext();
            var nhanvien = (from s in dataContext.NhanViens
                            where s.MaNV == MaNV
                            select s).Single();
            nhanvien.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
        }

        public ActionResult ThongTinCaNhan()
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var nhanvien = (from s in dataContext.NhanViens
                            where s.Username == username
                            select s).Single();
            return View(nhanvien);
        }

        [HttpPost]
        public ActionResult ThongTinCaNhan(int MaNV, string hoten, string cmnd, DateTime ngaysinh, string sodt)
        {
            // manage role ko quan tam
            if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                return RedirectToAction("Login", "Admin");
            helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role);
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var nhanvien = (from s in dataContext.NhanViens
                            where s.MaNV == MaNV
                            select s).Single();
            if (cmnd == null || cmnd == "" || sodt == null || sodt == "" || ngaysinh == null)
            {
                ViewBag.FailMessage = "Không được để trống các thông tin bắt buộc.";
                return View(nhanvien);
            }
            if (DateTime.Now.Year - ngaysinh.Year < 18)
            {
                ViewBag.FailMessage = "Ngày tháng năm sinh không hợp lệ. Tuổi nhân viên phải lớn hơn 18!";
                return View(nhanvien);
            }
            nhanvien.HoTenNV = hoten;
            nhanvien.CMND = cmnd;
            //DateTime ns = Convert.ToDateTime(ngaysinh);
            nhanvien.NgaySinh = ngaysinh;
            nhanvien.SoDT = sodt;
            dataContext.SubmitChanges();
            ViewBag.SuccessMessage = "Bạn cập nhật thông tin cá nhân thành công";
            return View(nhanvien);
        }
    }
}
