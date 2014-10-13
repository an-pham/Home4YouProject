using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.back.Controllers
{
    public class KhuyenMaiController : Controller
    {
        //
        // GET: /back/KhuyenMai/
        HelperController helper = new HelperController();
        string role;
        bool is_login;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session["Role"] != null)
            {
                role = HttpContext.Session["Role"].ToString();
                is_login = (bool)filterContext.HttpContext.Session["IsLogin"];
            }
        }
        public ActionResult Index()
        {
            return View();
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
            int total_rows = (from khuyenmai in dataContext.KhuyenMais select khuyenmai).Count();
             //Lấy tổng số dòng để phân trang
            if (title != null)
            {
                total_rows = (from khuyenmai in dataContext.KhuyenMais 
                              where khuyenmai.TenKhuyenMai.Contains(title)==true
                              select khuyenmai).Count();
            }
                           
            ViewBag.total_rows = total_rows;
            //End of Lấy tổng số dòng để phân trang

            string base_URL = "DanhSach"; //quan trong, ten action cua controller
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
            var DanhSachKhuyenMai = from km in dataContext.KhuyenMais select km;
            if (title != null)
            {
                DanhSachKhuyenMai = (from km in dataContext.KhuyenMais
                                    where km.TenKhuyenMai.Contains(title)==true
                                    select km);
            }
            else
            ViewBag.DanhSachKhuyenMai = DanhSachKhuyenMai;
            // kiem tra delete, edit
            foreach (var d in DanhSachKhuyenMai)
            {
                if (khong_xoa_sua(d.MaKM))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            return View(DanhSachKhuyenMai);
        }
        [HttpGet]
        public ActionResult Sua(int MaKM)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
			if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            var khuyenmai = (from KhuyenMai in dataContext.KhuyenMais
                             where KhuyenMai.MaKM == MaKM
                             select KhuyenMai).Single();
            return View(khuyenmai);
        }
        [HttpPost]
        public ActionResult Sua(int MaKM, string TenKM, string GhiChu, string tinhtrang)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
			if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            var khuyenmai = (from KhuyenMai in dataContext.KhuyenMais where KhuyenMai.MaKM == MaKM select KhuyenMai).Single();
            if (TenKM == null || TenKM == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View(khuyenmai);
            }
            khuyenmai.TenKhuyenMai = TenKM;
            khuyenmai.GhiChu = GhiChu;
            khuyenmai.TinhTrang = tinhtrang;
            try
            {
                dataContext.SubmitChanges();
                ViewBag.Message = "Cập nhật khuyến mãi thành công!";
            }
            catch
            {
                ViewBag.Message = "Cập nhật khuyến mãi thất bại!";
            }
            return View(khuyenmai);
        }
        [HttpGet]
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
        public ActionResult Them(string TenKM, string GhiChu, string tinhtrang)
        {
            //Kiểm tra xem đã đăng nhập hay chưa
            //Nếu chưa ==> Về trang đăng nhập
            if (!is_login)
                return RedirectToAction("Login", "Admin");
			if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            //End of Kiểm tra xem đã đăng nhập hay chưa
            var dataContext = new HomeForYouDataContext();
            if (TenKM == null || TenKM == "")
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                ViewBag.Success = 0;
                return View();
            }
            KhuyenMai km = new KhuyenMai();
            km.TenKhuyenMai = TenKM;
            km.GhiChu = GhiChu;
            km.TinhTrang = tinhtrang;
            km.Xoa = true;
            km.Sua = true;
            dataContext.KhuyenMais.InsertOnSubmit(km);
            try
            {
                dataContext.SubmitChanges();
                ViewBag.Message = "Thêm mới khuyến mãi thành công!";
                ViewBag.Success = 1;
            }
            catch
            {
                ViewBag.Message = "Thêm mới khuyến mãi thất bại!";
                ViewBag.Success = 0;
            }
            return View();
        }
		
        public void Xoa(int MaKM)
        {
            var dataContext = new HomeForYouDataContext();
            var km = (from KhuyenMai in dataContext.KhuyenMais where KhuyenMai.MaKM == MaKM select KhuyenMai).Single();
            km.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
            ViewBag.Message = "Xóa thành công khuyến mãi!";
        }
        //Kiểm tra khi xóa khuyến mãi
        public bool khong_xoa_sua(int MaKM)
        {
            var dataContext = new HomeForYouDataContext();
            if (dataContext.Deals.Any(deal => KhuyenMai.Equals(deal.KhuyenMai, MaKM)))
                return true;//Có tồn tại => không cho xóa
            return false;//Cho xóa
        }
       
    }
}
