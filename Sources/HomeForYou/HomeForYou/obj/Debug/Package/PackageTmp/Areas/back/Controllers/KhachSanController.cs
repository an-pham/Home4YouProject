using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace HomeForYou.Areas.back.Controllers
{
	public class DoanhThu
    {
        public string tenks;
        public float doanhthu;
    }

    public class LichSuDat
    {
        public string tenks;
        public int sodeal;
    }
	public class CT_Phong
    {
        public Phong phong;
        public List<DSTienNghi> tiennghi;
    }
    public class KhachSanController : Controller
    {
        //
        // GET: /back/KhachSan/
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
        HomeForYouDataContext datacontext = new HomeForYouDataContext();

        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }

        public ActionResult DanhSach(int? vung=0, int? thanhpho=0, int? quocgia=0)
        {
            // manage role ko quan tam
            /*    if (HttpContext.Session == null || HttpContext.Session["IsLogin"] == null || HttpContext.Session["Role"] == null)
                    return RedirectToAction("Login", "Admin");*/
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            int total_rows = 0;
            if (quocgia == 0)
            {
                total_rows = (from ks in datacontext.KhachSans
                              select ks).Count();
            }
            else
            {
                // Lua chon theo quoc gia
                if (thanhpho == 0)
                {
                    total_rows = (from ks in datacontext.KhachSans
                                  where ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia
                                  select ks).Count();
                }
                else
                {
                    // Lua chon theo quoc gia - thanh pho
                    if (vung == 0)
                    {
                        total_rows = (from ks in datacontext.KhachSans
                                      where ks.Vung1.ThanhPho1.MaTP == thanhpho
                                      select ks).Count();
                    }
                    // Lua cho theo quoc gia - thanh pho - vung
                    else
                    {
                        total_rows = (from ks in datacontext.KhachSans
                                      where ks.Vung1.MaVung == vung
                                      select ks).Count();
                    }
                }
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
            var khachsans = (from ks in datacontext.KhachSans
                             select ks).Skip(start).Take(offset);
            if (quocgia == 0)
            {
            }
            else
            {
                // Lua chon theo quoc gia
                if (thanhpho == 0)
                {
                    khachsans = (from ks in datacontext.KhachSans
                                 where ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia
                                 select ks).Skip(start).Take(offset);
                }
                else
                {
                    // Lua chon theo quoc gia - thanh pho
                    if (vung == 0)
                    {
                        khachsans = (from ks in datacontext.KhachSans
                                     where ks.Vung1.ThanhPho1.MaTP == thanhpho
                                     select ks).Skip(start).Take(offset);
                    }
                    // Lua cho theo quoc gia - thanh pho - vung
                    else
                    {
                        khachsans = (from ks in datacontext.KhachSans
                                     where ks.Vung1.MaVung == vung
                                     select ks).Skip(start).Take(offset);
                    }
                }
            }
            // kiem tra delete, edit
            foreach (var d in khachsans)
            {
                if (kiemtra_xoa_sua(d.MaKS))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            ViewBag.quocgias = (from qg in datacontext.QuocGias
                                select qg);
            ViewBag.thanhphos = (from tp in datacontext.ThanhPhos
                                 select tp);
            ViewBag.vungs = (from v in datacontext.Vungs
                                select v);
            return View(khachsans);
        }

        public bool kiemtra_xoa_sua(int id)
        {
            List<int> ds_deal = (from d in datacontext.Deals
                                 where d.TinhTrang.Contains("Enabled") == true && d.KhachSan == id
                                 select (int)d.MaDeal).ToList();
            List<int> ds_ctdeal_chuathanhtoan = (from ctd in datacontext.ChiTietDatDeals
                                   where ctd.TinhTrang.Contains("Unpaid") == true && ds_deal.Contains((int)ctd.MaDeal) == true
                                   select ctd.MaCT).ToList();            
            if (ds_ctdeal_chuathanhtoan.Count() != 0)
            {
                return true;    //khong cho xoa
            }
            return false;
        }
        
        public ActionResult Xoa(int id)
        {
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            KhachSan ks = datacontext.KhachSans.Single(_ks => _ks.MaKS == id);

            ks.TinhTrang = "Deleted";
            datacontext.SubmitChanges();

            return RedirectToAction("DanhSach");
        }

        [HttpGet]
        public ActionResult Sua(int id)
        {
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            KhachSan ks = datacontext.KhachSans.Single(_ks => _ks.MaKS == id);
            
            var vungs = (from v in datacontext.Vungs
                         where v.TinhTrang == "Enabled"
                         select v);
            ViewBag.vungs = vungs;
            //List<Vung> vungs = datacontext.Vungs.Where(v => v.TinhTrang.Contains("Enabled") == true).ToList();
            //ViewBag.vungs = new SelectList(vungs, "TenVung", "TenVung");
            var nhacungcaps = (from n in datacontext.NhaCungCaps
                               where n.TinhTrang == "Enabled"
                               select n);
            ViewBag.nhacungcaps = nhacungcaps;
            //List<NhaCungCap> nhacungcaps = datacontext.NhaCungCaps.Where(_ncc => _ncc.TinhTrang.Contains("Enabled") == true).ToList();
            //ViewBag.nhacungcaps = new SelectList(nhacungcaps, "TenNCC", "TenNCC");
            var hinh = (from s in datacontext.HinhKhachSans
                        join k in datacontext.KhachSans on s.MaKS equals k.MaKS
                        where k.MaKS == id
                        select s.TenHinh);
            ViewBag.TenHinh = hinh;
			
			var ds_phong = (from p in datacontext.Phongs
                            where p.KhachSan == id
                            select p);
            List<CT_Phong> CT_Phongs = new List<CT_Phong>();
            foreach (var s in ds_phong)
            {
                CT_Phong ctp = new CT_Phong();
                ctp.phong = s;
                List<DSTienNghi> tiennghi = (from ct in datacontext.ChiTietPhongs
                                join tn in datacontext.DSTienNghis on ct.ChiTiet equals tn.ID
                                where ct.MaPhong == s.MaPhong
                                select tn).ToList();
                ctp.tiennghi = tiennghi;
                CT_Phongs.Add(ctp);
            }
            ViewBag.DSTienNghi = (from tn in datacontext.DSTienNghis
                                  select tn).ToList();
            ViewBag.Phong = CT_Phongs;
            return View(ks);
        }

        [HttpPost]
        public ActionResult Sua(int id, string diachi, string mota, string sodt, string tenduong, string tinhtrang, int vung, int nhacungcap, string tenks, IEnumerable<HttpPostedFileBase> file, string txtCheckList, string hinhdaidien, string baidauxe, string dichvu, string internet, string tongquat, TimeSpan nhanphong, TimeSpan traphong)
        {
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            //Delete file
            if (txtCheckList!="")
            {
                List<string> check = new List<string>();
                string[] temp = txtCheckList.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in temp)
                {
                    check.Add(s);
                }
                var hks = (from h in datacontext.HinhKhachSans
                       where h.MaKS == id && check.Contains(h.TenHinh)
                       select h);
                datacontext.HinhKhachSans.DeleteAllOnSubmit(hks);
                char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                string filepath = "";
                // Set our full path for deleting
                foreach (var f in check)
                {
                    filepath = "/Content/img" + DirSeparator + f;
                    if (System.IO.File.Exists(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + filepath)))
                    {
                        // Delete our file
                        System.IO.File.Delete(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + filepath));
                    }
                    filepath = "";
                }
            }
            //ket thuc xoa hinh
            KhachSan __ks = datacontext.KhachSans.Single(_ks => _ks.MaKS == id);
           
            var vungs = (from v in datacontext.Vungs
                         where v.TinhTrang == "Enabled"
                         select v);
            ViewBag.vungs = vungs;
            var nhacungcaps = (from n in datacontext.NhaCungCaps
                               where n.TinhTrang == "Enabled"
                               select n);
            ViewBag.nhacungcaps = nhacungcaps;
            var hinh = (from s in datacontext.HinhKhachSans
                        join k in datacontext.KhachSans on s.MaKS equals k.MaKS
                        where k.MaKS == id
                        select s.TenHinh);
            ViewBag.TenHinh = hinh;
            if (vung == 0 || nhacungcap == 0 || tenduong == null || tenduong == "" || tenks == null || tenks == "" || diachi == null || diachi == "")
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                ViewBag.Success = 0;
                return View(__ks);
            }
            __ks.DiaChi = diachi;
            if (mota.Length != 0)
                __ks.MoTa = mota;
            if (hinhdaidien != "")
                __ks.HinhDaiDien = hinhdaidien;
            __ks.SoDT = sodt;
            __ks.TenDuong = tenduong;
            __ks.TinhTrang = tinhtrang;
			__ks.Vung = datacontext.Vungs.Single(v => v.MaVung.Equals(vung)==true).MaVung;
			__ks.NhaCungCap = datacontext.NhaCungCaps.Single(ncc => ncc.MaNCC.Equals(nhacungcap) == true).MaNCC;
            __ks.TenKS = tenks;
			__ks.BaiDauXe = baidauxe;
            __ks.DichVu = dichvu;
            __ks.Internet = internet;
            __ks.TongQuat = tongquat;
            __ks.NhanPhong = nhanphong;
            __ks.TraPhong = traphong;
            //upload hinh anh
            string fileName = "";
            if (file!=null)
            {
                foreach (HttpPostedFileBase uf in file)
                {
                    HttpPostedFileBase UploadedFile = uf;
                    if (uf!=null && uf.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(UploadedFile.FileName);
                        var count = datacontext.HinhKhachSans.Count(hks => (hks.TenHinh.Contains(fileName) == true && hks.MaKS==id));
                        if (count == 0)
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                            uf.SaveAs(path);
                            HinhKhachSan hinhks = new HinhKhachSan();
                            hinhks.MaKS = id;
                            hinhks.TenHinh = fileName;
                            datacontext.HinhKhachSans.InsertOnSubmit(hinhks);
                        }
                    }
                }
            }
            //ket thuc upload hinh
            datacontext.SubmitChanges();
           
            ViewBag.Message = "Bạn cập nhật " + __ks.TenKS + " thành công.";
            ViewBag.Success = 1;
            //return View(__ks);
			return RedirectToAction("Sua", __ks.MaKS);
        }

        [HttpGet]
        public ActionResult Them()
        {
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            List<Vung> vungs = datacontext.Vungs.Where(v => v.TinhTrang.Contains("Enabled") == true).ToList();
            ViewBag.vungs = new SelectList(vungs, "TenVung", "TenVung");

            List<NhaCungCap> nhacungcaps = datacontext.NhaCungCaps.Where(_ncc => _ncc.TinhTrang.Contains("Enabled") == true).ToList();
            ViewBag.nhacungcaps = new SelectList(nhacungcaps, "TenNCC", "TenNCC");

            return View();
        }

        [HttpPost]
        public ActionResult Them(string diachi, string mota, string sodt, string tenduong, string vung, string nhacungcap, string tenks, string tongquat, string dichvu, string internet, string baidauxe, TimeSpan nhanphong, TimeSpan traphong)
        {
            
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            List<Vung> vungs = datacontext.Vungs.Where(v => v.TinhTrang.Contains("Enabled") == true).ToList();
            ViewBag.vungs = new SelectList(vungs, "TenVung", "TenVung");

            List<NhaCungCap> nhacungcaps = datacontext.NhaCungCaps.Where(_ncc => _ncc.TinhTrang.Contains("Enabled") == true).ToList();
            ViewBag.nhacungcaps = new SelectList(nhacungcaps, "TenNCC", "TenNCC");
            if (tenks == "" || vung == "" || nhacungcap == ""||tenduong=="")
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc";
                ViewBag.Success = 0;
                return View();
            }
            KhachSan ks = new KhachSan();
            ks.TenKS = tenks;
            ks.DiaChi = diachi;
            ks.MoTa = mota;
            ks.NhaCungCap = datacontext.NhaCungCaps.Single(ncc => ncc.TenNCC.Contains(nhacungcap) == true).MaNCC;
            ks.Vung = datacontext.Vungs.Single(v => v.TenVung.Contains(vung) == true).MaVung;
            ks.SoDT = sodt;
            ks.TenDuong = tenduong;
            ks.TinhTrang = "Enable";
			ks.BaiDauXe = baidauxe;
            ks.DichVu = dichvu;
            ks.Internet = internet;
            ks.TongQuat = tongquat;
            ks.NhanPhong = nhanphong;
            ks.TraPhong = traphong;
            ks.Xoa = true;
            ks.Sua = true;
            datacontext.KhachSans.InsertOnSubmit(ks);
            datacontext.SubmitChanges();
            ViewBag.Message = "Đã thêm khách sạn " + tenks + " thành công";
            ViewBag.Success = 1;
            return View();
        }
        public ActionResult ThongKeDoanhThu(DateTime? batdau = null, DateTime? ketthuc = null)
        {
			// manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
			if (batdau == null)
            {
                batdau = DateTime.MinValue.Date;
            }
            if (ketthuc == null)
            {
                ketthuc = DateTime.Today;
            }
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
                         group new {ks, d, ctd, hd} by ks.TenKS into kq
                         select new DoanhThu { tenks = kq.Key, doanhthu = (float)kq.Sum(p =>p.hd.TongTien)}).ToList();
			ViewBag.result = result;
            ViewBag.total_rows = total_rows;
            return View();     
        }

		public ActionResult ThongKeLichSuDatDeal(DateTime? batdau = null, DateTime? ketthuc=null)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam

            if (batdau == null)
            {
                batdau = DateTime.MinValue.Date;
            }
            if (ketthuc == null)
            {
                ketthuc = DateTime.Today;
            }
            int total_rows = (from ks in datacontext.KhachSans
                              join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                              join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                              join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                              where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true
                              group new { ks, d, ctd, hd } by ks.TenKS into kq
                              select new LichSuDat { tenks = kq.Key, sodeal = kq.Count(p => p.ctd.TinhTrang.Contains("Enabled")) }).Count();
            var result = (from ks in datacontext.KhachSans
                          join d in datacontext.Deals on ks.MaKS equals d.KhachSan
                          join ctd in datacontext.ChiTietDatDeals on d.MaDeal equals ctd.MaDeal
                          join hd in datacontext.HoaDons on ctd.MaCT equals hd.CTDeal
                          where hd.NgayThanhToan >= batdau && hd.NgayThanhToan < ketthuc && hd.TinhTrang.Contains("Enabled") == true
                          group new { ks, d, ctd, hd } by ks.TenKS into kq
                          select new LichSuDat { tenks = kq.Key, sodeal = kq.Count(p => p.ctd.TinhTrang.Contains("Enabled")) }).ToList();

            ViewBag.result = result;
            ViewBag.total_rows = total_rows;
            return View();
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

        [HttpPost]
        public JsonResult DanhSach_Vung(int ma_thanh_pho)
        {
            var dataContext = new HomeForYouDataContext();
            var ds_vung = (from v in dataContext.Vungs
                           where v.ThanhPho == ma_thanh_pho
                           select new { ma_vung = v.MaVung, ten_vung = v.TenVung });
            return Json(ds_vung);
        }
		
		[HttpGet]
        public ActionResult SuaPhong(int id)
        {
            CT_Phong ctp = new CT_Phong();
            ctp.phong = (from ph in datacontext.Phongs
                             where ph.MaPhong == id
                             select ph).Single();


            List<DSTienNghi> tiennghi = (from ct in datacontext.ChiTietPhongs
                                         join tn in datacontext.DSTienNghis on ct.ChiTiet equals tn.ID
                                         where ct.MaPhong == id
                                         select tn).ToList();
            ctp.tiennghi = tiennghi;
            ViewBag.Phong = ctp;
            ViewBag.TienNghi = (from tn in datacontext.DSTienNghis
                                select tn);
            return View();
        }

        [HttpPost]
        public ActionResult SuaPhong(int idks, int id, string loaiphong, double dongia, int soluong, string txtCheckList, string tinhtrang)
        {
            Phong p = (from ph in datacontext.Phongs
                       where ph.MaPhong == id
                       select ph).Single();
            p.LoaiPhong = loaiphong;
            p.DonGia = dongia;
            p.SoLuong = soluong;
            p.TinhTrang = tinhtrang;
            List<ChiTietPhong> ctp = (from _ctp in datacontext.ChiTietPhongs
                                      where _ctp.MaPhong == id
                                      select _ctp).ToList();
            if (txtCheckList != "")
            {
                datacontext.ChiTietPhongs.DeleteAllOnSubmit(ctp);
                List<string> check = new List<string>();
                string[] temp = txtCheckList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in temp)
                {
                    check.Add(s);
                }
                foreach (var f in check)
                {
                    ChiTietPhong _ctp = new ChiTietPhong();
                    _ctp.MaPhong = id;
                    _ctp.ChiTiet = Convert.ToInt32(f);
                    datacontext.ChiTietPhongs.InsertOnSubmit(_ctp);
                }
            }
            datacontext.SubmitChanges();
            return RedirectToAction("Sua", new { id = idks});
        }

        public void XoaPhong(int id)
        {
            Phong p = (from phong in datacontext.Phongs
                           where phong.MaPhong == id
                           select phong).Single();
            p.TinhTrang = "Deleted";
            datacontext.SubmitChanges();
        }

        [HttpGet]
        public ActionResult ThemPhong(int idks)
        {
            ViewBag.KhachSan = (from ks in datacontext.KhachSans
                                    where ks.MaKS == idks
                                    select ks).Single();
            List<DSTienNghi> tiennghi = (from ct in datacontext.ChiTietPhongs
                                         join tn in datacontext.DSTienNghis on ct.ChiTiet equals tn.ID
                                         where ct.MaPhong == idks
                                         select tn).ToList();
            ViewBag.TienNghi = (from tn in datacontext.DSTienNghis
                                select tn);
            return View();
        }

        [HttpPost]
        public ActionResult ThemPhong(string loaiphong, int soluong, int idks, double dongia, string txtCheckList)
        {
            Phong phong = new Phong();
            phong.LoaiPhong = loaiphong;
            phong.SoLuong = soluong;
            phong.KhachSan = idks;
            phong.DonGia = dongia;
            phong.Xoa = true;
            phong.Sua = true;
            datacontext.Phongs.InsertOnSubmit(phong);
            datacontext.SubmitChanges();

            Phong ph = datacontext.Phongs.OrderByDescending(_ph => _ph.MaPhong).Take(1).Single();

            int id = ph.MaPhong;
            if (txtCheckList != "")
            {
                List<string> check = new List<string>();
                string[] temp = txtCheckList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in temp)
                {
                    check.Add(s);
                }
                foreach (var f in check)
                {
                    ChiTietPhong _ctp = new ChiTietPhong();
                    _ctp.MaPhong = id; 
                    _ctp.ChiTiet = Convert.ToInt32(f);
                    datacontext.ChiTietPhongs.InsertOnSubmit(_ctp);
                }
            }
            datacontext.SubmitChanges();

            return RedirectToAction("Sua", new { id = idks });
        }
    }
}
