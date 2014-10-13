using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HomeForYou.Areas.back.Controllers
{
    public class DealKSController : Controller
    {
        //
        // GET: /back/Deal/
        HelperController helper = new HelperController();
        string role;
		int ma_nv;
        bool is_login;
		public class LichSuDeal
        {
            public ChiTietDatDeal ct_deal;
            public KhachHang khachhang;
        }
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            if (ctx.HttpContext.Session["Role"] != null)
            {
                role = ctx.HttpContext.Session["Role"].ToString();
                is_login = (bool)ctx.HttpContext.Session["IsLogin"];
				ma_nv = (int)Session["Staff_ID"];
            }
            //role = ControllerContext.HttpContext.Session["Role"].ToString();
        }

        public void Index()
        {
            Response.Redirect("DanhSach");
        }

        public ActionResult DanhSach(int? country=null, int? city=null, int? area=null, int? hotel = null, int? month = null)
        {
            // manage role ko quan tam
            /*    if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                    return RedirectToAction("Login", "Admin");*/
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            ViewBag.TimeNow = DateTime.Now.Date.ToString("d/MM/yyyy");
            var dataContext = new HomeForYouDataContext();
            int total_rows = 0;
            if (hotel != null && month != null)
            {
                total_rows = (from d in dataContext.Deals
                                join km in dataContext.KhuyenMais on d.KhuyenMai equals km.MaKM
                                where (d.NgayBatDau.Value.Month == month || d.NgayKetThuc.Value.Month == month) && d.KhachSan == hotel
                                select d).Count();
            }
            total_rows = (from d in dataContext.Deals
                              join km in dataContext.KhuyenMais on d.KhuyenMai equals km.MaKM
                              select d).Count();
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
            if (hotel != null && month != null && hotel!=0 && month!=0)//tim theo khach san va thang
            {
                var deals = (from d in dataContext.Deals
                             join km in dataContext.KhuyenMais on d.KhuyenMai equals km.MaKM
                             where (d.NgayBatDau.Value.Month == month || d.NgayKetThuc.Value.Month == month) && d.KhachSan == hotel
                             select d).Skip(start).Take(offset);
                // kiem tra delete, edit
                foreach (var d in deals)
                {
                    if (khong_xoa_sua(d))
                    {
                        d.Xoa = false;
                        d.Sua = false;
                    }
                }
                var countries = (from c in dataContext.QuocGias
                                 select c);
                ViewBag.Countries = countries;
                return View(deals);

            }
            var deal = (from d in dataContext.Deals
                     join km in dataContext.KhuyenMais on d.KhuyenMai equals km.MaKM
                     where ((country!=0 && country!=null && d.KhachSan1.Vung1.ThanhPho1.QuocGia1.MaQG==country)||(country==0 || country==null))
                           && ((city != 0 && city != null && d.KhachSan1.Vung1.ThanhPho1.MaTP == city) || (city==0 || city==null))
                           && ((area != 0 && area != null && d.KhachSan1.Vung1.MaVung == area) || (area==0 || area==null))
                     select d).Skip(start).Take(offset);
            // kiem tra delete, edit
            foreach (var d in deal)
            {
                if (khong_xoa_sua(d))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            var countrie = (from c in dataContext.QuocGias
                             select c);
            ViewBag.Countries = countrie;
            if (hotel != null && hotel!=0)
                ViewBag.hotel = dataContext.KhachSans.Where(ks => ks.MaKS == hotel).Single().TenKS.ToString();
            else
                ViewBag.hotel = "";
            if (country != null && country != 0)
                ViewBag.country = dataContext.QuocGias.Where(qg => qg.MaQG == country).Single().TenQG.ToString();
            else
                ViewBag.country = "";
            if (city != null && city!=0)
                ViewBag.city = dataContext.ThanhPhos.Where(tp => tp.MaTP == city).Single().TenTP.ToString();
            else
                ViewBag.city = "";
            if (area != null && area!=0)
                ViewBag.area = dataContext.Vungs.Where(v => v.MaVung == area).Single().TenVung.ToString();
            else
                ViewBag.area = "";
            return View(deal);
        }
		
		[HttpGet]
        public ActionResult Sua(int MaDeal)
        {
            var dataContext = new HomeForYouDataContext();
            var deal = (from d in dataContext.Deals
                        where d.MaDeal == MaDeal
                        select d).Single();
            var khuyenmai = (from k in dataContext.KhuyenMais
                             select k);
            var phong = (from p in dataContext.Phongs
                         select p);
            ViewBag.KhuyenMai = khuyenmai;
            ViewBag.Phong = phong;
            return View(deal);
        }
        [HttpPost]
        public ActionResult Sua(int MaDeal, float Gia, float Tien_Cong_Them, int Phong, DateTime NgayBD, DateTime NgayKT, int KhuyenMai, string tinhtrang)
        {
            // manage role ko quan tam
            if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                return RedirectToAction("Login", "Admin");
            helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role);
            // manage role ko quan tam

            var dataContext = new HomeForYouDataContext();
            var deal = (from d in dataContext.Deals
                        where d.MaDeal == MaDeal
                        select d).Single();
            if (Gia == null || NgayBD == null || NgayKT == null)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return View(deal);
            }
            deal.TienCongThem = Tien_Cong_Them;
            deal.Phong = Phong;
            deal.NgayBatDau = NgayBD;
            deal.NgayKetThuc = NgayKT;
            deal.KhuyenMai = KhuyenMai;
            deal.TinhTrang = tinhtrang;
            dataContext.SubmitChanges();

            var khuyenmai = (from k in dataContext.KhuyenMais
                             select k);
            var phong = (from p in dataContext.Phongs
                         select p);
            ViewBag.KhuyenMai = khuyenmai;
            ViewBag.Phong = phong;
            ViewBag.Message = "Bạn cập nhật deal thành công";
            return View(deal);
        }
        public ActionResult ChiTiet(int MaDeal, int? huy=null, int? tt = null)
        {
            var dataContext = new HomeForYouDataContext();
            var deal = (from d in dataContext.Deals
                        where d.MaDeal == MaDeal
                        select d).Single();
            var ls_deal = (from ctd in dataContext.ChiTietDatDeals
                           where ctd.MaDeal == MaDeal
                           select ctd);
            List<LichSuDeal> ct_deal_list = new List<LichSuDeal>();
            foreach (var l in ls_deal)
            {
                LichSuDeal lsd = new LichSuDeal();
                lsd.ct_deal = l; 
                var kh = (from k in dataContext.KhachHangs
                          where k.ID == l.MaKH
                          select k
                          ).Single();
                lsd.khachhang = kh;
                ct_deal_list.Add(lsd);
            }
            ViewBag.LichSuDatDeal = ct_deal_list;
            if (huy == 1)
            {
                ViewBag.Message = "Hủy deal thành công.";
            }
            if (tt==1)
            {
                ViewBag.Message = "Thanh toán deal thành công.";
            }
            return View(deal);
        }

        [HttpGet]
        public ActionResult Them()
        {
            var dataContext = new HomeForYouDataContext();
            var khachsan = (from k in dataContext.KhachSans
                            select k);
            var khuyenmai = (from km in dataContext.KhuyenMais
                             select km);
            ViewBag.KhachSan = khachsan;
            ViewBag.KhuyenMai = khuyenmai;
            return View();
        }
        [HttpPost]
        public ActionResult Them(int KhachSan, int Phong, int KhuyenMai, float? Gia=null, float? Tien_Cong_Them=null, DateTime? NgayBD=null, DateTime? NgayKT=null)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            if (KhachSan == 0 || Phong == 0 || NgayBD == null || NgayKT == null||Gia==null||Tien_Cong_Them==null)
            {
                var khachsan1 = (from k in dataContext.KhachSans
                                select k);
                var khuyenmai1 = (from km in dataContext.KhuyenMais
                                 select km);
                ViewBag.KhachSan = khachsan1;
                ViewBag.KhuyenMai = khuyenmai1;
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                ViewBag.Success = 0;
                return View();
            }
            Deal deal = new Deal();
            deal.Gia = Gia;
            deal.TienCongThem = Tien_Cong_Them;
            deal.KhachSan = KhachSan;
            deal.Phong = Phong;
            deal.NgayBatDau = NgayBD;
            deal.NgayKetThuc = NgayKT;
            deal.Xoa = true;
            deal.Sua = true;
            if (KhuyenMai!=0 && KhuyenMai!=null)
                deal.KhuyenMai = KhuyenMai;
            deal.NhanVienTao = ma_nv;

            dataContext.Deals.InsertOnSubmit(deal);
            dataContext.SubmitChanges();

            var khachsan = (from k in dataContext.KhachSans
                            select k);
            var khuyenmai = (from km in dataContext.KhuyenMais
                             select km);
            ViewBag.KhachSan = khachsan;
            ViewBag.KhuyenMai = khuyenmai;
            ViewBag.Message = "Bạn đã thêm deal thành công";
            ViewBag.Success = 1;
            return View();
        }
        public bool khong_xoa_sua(Deal deal)
        {
            foreach (var c in deal.ChiTietDatDeals)
            {
                if (c.TinhTrang == "Unpaid")
                {
                    return true;
                }
            }
             return false;
        }
  
        [HttpPost]
        public JsonResult DanhSach_ThanhPho(int ma_quoc_gia)
        {
            var dataContext = new HomeForYouDataContext();
            var ds_tp = (from t in dataContext.ThanhPhos
                         where t.QuocGia == ma_quoc_gia
                         select new { ma_tp = t.MaTP, ten_tp = t.TenTP });
            return Json(ds_tp);
        }

		public void HuyThanhToanDeal(int MaCT, int MaDeal)
        {
            var dataContext = new HomeForYouDataContext();
            var ct_deal = (from ct in dataContext.ChiTietDatDeals
                            where ct.MaCT == MaCT
                            select ct).Single();
            ct_deal.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
            var t = "ChiTiet?MaDeal=" + MaDeal.ToString() + "&huy=1";
            Response.Redirect(t);
        }
        public void ThanhToanDeal(int MaCT, int MaDeal)
        {
            var dataContext = new HomeForYouDataContext();
            var ct_deal = (from ct in dataContext.ChiTietDatDeals
                           where ct.MaCT == MaCT
                           select ct).Single();
            ct_deal.TinhTrang = "Paid";
            dataContext.SubmitChanges();
            //var t = "ChiTiet?MaDeal=" + MaDeal.ToString();
            var t = "ChiTiet?MaDeal=" + MaDeal.ToString() + "&tt=1";
            Response.Redirect(t);
        }
        [HttpPost]
        public JsonResult DanhSach_Vung(int ma_thanh_pho)
        {
            var dataContext = new HomeForYouDataContext();
            var ds_vung = (from v in dataContext.Vungs
                           where v.ThanhPho == ma_thanh_pho
                           select new { ma_vung = v.MaVung, ten_vung = v.TenVung });
            return Json(ds_vung);
        }

        [HttpPost]
        public JsonResult DanhSach_KhachSan(int ma_vung)
        {
            var dataContext = new HomeForYouDataContext();
            var ds_khachsan = (from k in dataContext.KhachSans
                           where k.Vung == ma_vung
                           select new { ma_ks = k.MaKS, ten_ks = k.TenKS });
            return Json(ds_khachsan);
        }

		[HttpPost]
        public JsonResult DanhSach_Phong(int ma_ks)
        {
            var dataContext = new HomeForYouDataContext();
            var ds_phong = (from p in dataContext.Phongs
                           where p.KhachSan == ma_ks
                           select new { ma_phong = p.MaPhong, ten_phong = p.LoaiPhong });
            return Json(ds_phong);
        }
    }
}
