using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace HomeForYou.Areas.back.Controllers
{
    public class BaoCaoController : Controller
    {
        //
        // GET: /back/BaoCao/
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
        // GET: /back/BaoCao/
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }
        public ActionResult BaoCao(string _batdau, string _ketthuc, int? khachsan=0)
        {
            var datacontext = new HomeForYouDataContext();
			ViewBag.pre_batdau = _batdau;
            ViewBag.pre_ketthuc = _ketthuc;
			ViewBag.pre_khachsan = khachsan;
            if (_batdau == null ||_batdau=="") // Xử lý trường hợp không nhập
            {
                _batdau = "01-01-0001";
            }
            if (_ketthuc == null || _ketthuc=="") // Xử lý trường hợp không nhập
            {
                _ketthuc = "01-01-0001";
            }
            DateTime batdau = Convert.ToDateTime(_batdau);
            DateTime ketthuc = Convert.ToDateTime(_ketthuc);
            var ksan = from s in datacontext.KhachSans
                       where s.TinhTrang == "Enabled"
                       select s;
            //ViewData["KhachSan"] = new SelectList(ksan, "MaKS", "TenKS");
            ViewBag.khachsan = ksan;
            if (batdau > ketthuc)
            {
                ViewBag.total_rows = 0;
                ViewBag.FailMessage = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc.";
                return View();
            }
            DateTime tmp;
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
           
            if (batdau == Convert.ToDateTime("01-01-0001"))
            {
                batdau = DateTime.MinValue.Date;
            }
            else
            {
                tmp = (DateTime)batdau;
            }
            if (ketthuc == Convert.ToDateTime("01-01-0001"))
            {
                ketthuc = DateTime.Today;
            }
            if (khachsan == 0)
            {
                int total_rows = (from ks in datacontext.KhachSans
                                  join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                                  join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                                  join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                                  where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true
                                  group new { ks, d, ctd, hd } by ks.TenKS into kq
                                  select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).Count();
                var result = (from ks in datacontext.KhachSans
                              join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                              join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                              join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                              where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true
                              group new { ks, d, ctd, hd } by ks.TenKS into kq
                              select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).ToList();
                ViewBag.result = result;
                ViewBag.total_rows = total_rows;
                return View();
            }
            else
            {
                int total_rows = (from ks in datacontext.KhachSans
                                  join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                                  join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                                  join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                                  where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true && ks.MaKS == khachsan
                                  group new { ks, d, ctd, hd } by ks.TenKS into kq
                                  select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).Count();
                var result = (from ks in datacontext.KhachSans
                              join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                              join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                              join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                              where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true && ks.MaKS == khachsan
                              group new { ks, d, ctd, hd } by ks.TenKS into kq
                              select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).ToList();
                ViewBag.result = result;
                ViewBag.total_rows = total_rows;
                return View();
            }
            //select ks.TenKS, SUM(hd.TongTien) as DoanhThu
            //from KhachSan ks join Deal d on ks.TenKS = d.KhachSan
            //join ChiTietDatDeal ctd on ctd.MaDeal = d.MaDeal
            //join HoaDon hd on hd.CTDeal = ctd.MaDeal
            //group by ks.TenKS 
        }
		
		[HttpPost]
        public JsonResult layDuLieu()
        {
            HomeForYouDataContext datacontext = new HomeForYouDataContext();
            
                int total_rows = (from ks in datacontext.KhachSans
                                  join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                                  join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                                  join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                                  group new { ks, d, ctd, hd } by ks.TenKS into kq
                                  select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).Count();
                var result = (from ks in datacontext.KhachSans
                              join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                              join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                              join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                              group new { ks, d, ctd, hd } by ks.TenKS into kq
                              select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).ToList();
                ViewBag.result = result;
                ViewBag.total_rows = total_rows;
                return Json(result);
        }

        [HttpPost]
        public JsonResult layDuLieuMotKhachSan(string _batdau, string _ketthuc, int khachsan)
        {
            HomeForYouDataContext datacontext = new HomeForYouDataContext();
            if (_batdau == null) // Xử lý trường hợp không nhập
            {
                _batdau = "01-01-0001";
            }
            if (_ketthuc == null) // Xử lý trường hợp không nhập
            {
                _ketthuc = "01-01-0001";
            }
            DateTime batdau = Convert.ToDateTime(_batdau);
            DateTime ketthuc = Convert.ToDateTime(_ketthuc);

            int total_rows = 1;
            var result = (from ks in datacontext.KhachSans
                          join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                          join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                          join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                          where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true && ks.MaKS == khachsan
                          group new { ks, d, ctd, hd } by new { thang = System.Convert.ToDateTime(hd.NgayThanhToan).Month.ToString(), nam = System.Convert.ToDateTime(hd.NgayThanhToan).Year.ToString() } into kq
                          select new { thang = (kq.Key.thang + '-' + kq.Key.nam).ToString(), doanhthu = (float)kq.Sum(p => p.hd.TongTien) }).ToList();
            ViewBag.result = result;
            ViewBag.total_rows = total_rows;
            return Json(result);
        }
    }
}
