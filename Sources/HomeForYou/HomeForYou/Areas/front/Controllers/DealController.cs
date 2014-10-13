using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HomeForYou.Areas.front.Controllers
{
    public class DealController : Controller
    {
        //
        // GET: /front/Deal/
        HomeForYouDataContext datacontext = new HomeForYouDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChiTiet(string slug)
        {
            var dataContext = new HomeForYouDataContext();
            KhachSan ks = (from _ks in datacontext.KhachSans
                           where _ks.Slug == slug
                           select _ks).SingleOrDefault();
            var quocgias = (from qg in dataContext.QuocGias
                            select qg);
            ViewBag.quocgias = quocgias;
            List<HinhKhachSan> hinhks = (from h in datacontext.HinhKhachSans
                                         where h.MaKS == ks.MaKS
                                         select h).ToList();
            var count = (from h in datacontext.HinhKhachSans
                         where h.MaKS == ks.MaKS
                         select h).Count();
            ViewBag.count = count;
            ViewBag.hinhks = hinhks;
            //Dem so luong khach san
            return View(ks);
        }
        [HttpGet]
        public ActionResult TimKiem(int? quocgia = 0, int? makm = 0)
        {
            var dataContext = new HomeForYouDataContext();
            var quocgias = (from qg in dataContext.QuocGias
                            select qg);
            ViewBag.quocgias = quocgias;
            List<KhachSan> khachsans = new List<KhachSan>();
            khachsans = datacontext.KhachSans.ToList();
            if ((quocgia != 0 && quocgia != null) || (makm != 0 && makm != null))
            {
                //int quocgia = Convert.ToInt32(var);

                if (quocgia != 0 && quocgia != null)
                {
                    khachsans = datacontext.KhachSans.Where(ks => ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia).ToList();
                }
                if (makm != 0 && makm != null)
                {
                    khachsans = (from s in datacontext.KhachSans
                                 join d in datacontext.Deals on s.MaKS equals d.KhachSan
                                 where d.KhuyenMai == makm
                                 select s).ToList();
                    ViewBag.KhuyenMai = datacontext.KhuyenMais.Where(km => km.MaKM == makm).Single();
                }

            }
            List<Phong> phongs = dataContext.Phongs.ToList();
            //foreach (var p in phongs)
            //{
            //    p.DonGia = p.DonGia * 1000;
            //}
            ViewBag.Phongs = phongs;
            ViewBag.Count = khachsans.Count();
            ViewBag.Diemden = "";
            ViewBag.Quocgia = "";
            ViewBag.Thanhpho = "";
            ViewBag.Vung = "";
            if (quocgia != null && quocgia != 0)
            {
                ViewBag.Quocgia = khachsans[0].Vung1.ThanhPho1.QuocGia1.TenQG;
            }

            //Dem so luong khach san
            var count0 = dataContext.KhachSans.Where(ks => (ks.Loai == null) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count0 = count0;
            var count1 = dataContext.KhachSans.Where(ks => (ks.Loai == 1) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count1 = count1;
            var count2 = dataContext.KhachSans.Where(ks => (ks.Loai == 2) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count2 = count2;
            var count3 = dataContext.KhachSans.Where(ks => (ks.Loai == 3) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count3 = count3;
            var count4 = dataContext.KhachSans.Where(ks => (ks.Loai == 4) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count4 = count4;
            var count5 = dataContext.KhachSans.Where(ks => (ks.Loai == 5) && (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia)).Count();
            ViewBag.count5 = count5;

            ////phan trang 
            //string base_URL = "TimKiem"; //quan trong, ten action cua controller
            //int total_rows = khachsans.Count();
            //ViewBag.total_rows = total_rows;
            //string URL_segment;
            //try
            //{
            //    URL_segment = Request.Url.Segments[4];
            //}
            //catch (Exception)
            //{
            //    URL_segment = null;
            //}
            //Libraries.Pagination pagination = new Libraries.Pagination(base_URL, URL_segment, total_rows);
            //string pageLinks = pagination.GetPageLinks();
            //int start = (pagination.CurPage - 1) * pagination.PerPage;
            //int offset = pagination.PerPage;
            //if (URL_segment != null)
            //    pageLinks = pageLinks.Replace(base_URL + "/", "");
            //ViewBag.pageLinks = pageLinks;
            ////phan trang
            khachsans = khachsans.Where(ks => ks.TinhTrang == "Enabled").ToList();
            return View(khachsans);

        }

        [HttpPost]
        public ActionResult TimKiem(string diemden, string optionsRadios, int giatoida, DateTime? start_datealt = null, DateTime? end_datealt = null, int? quocgia = 0, int? thanhpho = 0, int? vung = 0, int? hdgia = null, int? hdsao = null)
        {
            DateTime? start_date = start_datealt;
            DateTime? end_date = end_datealt;
            var dataContext = new HomeForYouDataContext();
            List<KhachSan> khachsans = new List<KhachSan>();
            DateTime ngaybd = new DateTime();
            DateTime ngaykt = new DateTime();
            if (start_date != null)
                ngaybd = Convert.ToDateTime(start_date);
            if (end_date != null)
                ngaykt = Convert.ToDateTime(end_date);
            //tinh ngay bd va ngay kt
            if (start_date == null && end_date != null)
            {
                ngaybd = ngaykt.AddDays(-1);
            }
            if (start_date != null && end_date == null)
            {
                ngaykt = ngaybd.AddDays(1);
            }

            //kiem tra lua chon
            switch (optionsRadios)
            {
                case "option1":
                    if (start_date == null && end_date == null)
                    {
                        switch (giatoida)
                        {
                            case 0:
                                khachsans = dataContext.KhachSans.Where(ks => (ks.TenKS.Contains(diemden) == true)
                                    || (ks.Vung1.ThanhPho1.TenTP.Contains(diemden) == true)).ToList();
                                break;
                            case 300:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia <= 300000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 500:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia > 300000 && p.DonGia <= 500000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 1000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia > 500000 && p.DonGia <= 1000000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 1500:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia > 1000000 && p.DonGia <= 1500000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 2000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia > 1500000 && p.DonGia <= 2000000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 20000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             where p.DonGia > 2000000 && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else //co gia tri ngay thang
                    {
                        switch (giatoida)
                        {
                            case 0:
                                khachsans = (from d in dataContext.Deals
                                             join ks in dataContext.KhachSans on d.KhachSan equals ks.MaKS
                                             where d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 300:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia <= 300000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 500:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia > 300000 && p.DonGia <= 500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 1000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia > 500000 && p.DonGia <= 1000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 1500:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia > 1000000 && p.DonGia <= 1500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 2000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia > 1500000 && p.DonGia <= 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            case 20000:
                                khachsans = (from p in dataContext.Phongs
                                             join ks in dataContext.KhachSans on p.KhachSan equals ks.MaKS
                                             join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                             where p.DonGia > 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt && (ks.TenKS.Contains(diemden) || ks.Vung1.ThanhPho1.TenTP.Contains(diemden))
                                             select ks).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "option2":
                    if (thanhpho == 0 && vung == 0) //tim theo quoc gia
                    {
                        if (start_date == null && end_date == null) //khong tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 where ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia <= 300000)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 300000 && p.DonGia <= 500000)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 500000 && p.DonGia <= 1000000)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 1000000 && p.DonGia <= 1500000)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 1500000 && p.DonGia <= 2000000)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 2000000)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                        else //tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia <= 300000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 300000 && p.DonGia <= 500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 500000 && p.DonGia <= 1000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 1000000 && p.DonGia <= 1500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 1500000 && p.DonGia <= 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.QuocGia1.MaQG == quocgia && p.DonGia > 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                    }
                    else if (vung == 0)//tim theo thanh pho
                    {
                        if (start_date == null && end_date == null) //khong tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 where ks.Vung1.ThanhPho1.MaTP == thanhpho
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia <= 300000)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 300000 && p.DonGia <= 500000)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 500000 && p.DonGia <= 1000000)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 1000000 && p.DonGia <= 1500000)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 1500000 && p.DonGia <= 2000000)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 2000000)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                        else //tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where ks.Vung1.ThanhPho1.MaTP == thanhpho && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia <= 300000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 300000 && p.DonGia <= 500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 500000 && p.DonGia <= 1000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 1000000 && p.DonGia <= 1500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 1500000 && p.DonGia <= 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.ThanhPho1.MaTP == thanhpho && p.DonGia > 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                    }
                    else //tim theo vung
                    {
                        if (start_date == null && end_date == null) //khong tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 where ks.Vung1.MaVung == vung
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia <= 300000)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 300000 && p.DonGia <= 500000)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 500000 && p.DonGia <= 1000000)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 1000000 && p.DonGia <= 1500000)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 1500000 && p.DonGia <= 2000000)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 2000000)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                        else //tim theo ngay
                        {
                            switch (giatoida)
                            {
                                case 0:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where ks.Vung1.MaVung == vung && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt
                                                 select ks).ToList();
                                    break;
                                case 300:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia <= 300000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 300000 && p.DonGia <= 500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 500000 && p.DonGia <= 1000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 1500:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 1000000 && p.DonGia <= 1500000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 2000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 150000 && p.DonGia <= 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                                case 20000:
                                    khachsans = (from ks in dataContext.KhachSans
                                                 join p in dataContext.Phongs on ks.MaKS equals p.KhachSan
                                                 join d in dataContext.Deals on ks.MaKS equals d.KhachSan
                                                 where (ks.Vung1.MaVung == vung && p.DonGia > 2000000 && d.NgayBatDau <= ngaybd && d.NgayKetThuc >= ngaykt)
                                                 select ks).ToList();
                                    break;
                            }
                        }
                    }
                    break;
            }
            List<Phong> phongs = dataContext.Phongs.ToList();
            //foreach (var p in phongs)
            //{
            //    p.DonGia = p.DonGia * 1000;
            //}
            ViewBag.Phongs = phongs;
            ViewBag.Count = khachsans.Count();
            ViewBag.Diemden = diemden;
            ViewBag.Quocgia = "";
            ViewBag.Thanhpho = "";
            ViewBag.Vung = "";
            if (khachsans.Count != 0)
            {
                if (quocgia != null && quocgia != 0)
                {
                    ViewBag.Quocgia = khachsans[0].Vung1.ThanhPho1.QuocGia1.TenQG;
                }
                if (thanhpho != null && thanhpho != 0)
                {
                    ViewBag.Thanhpho = khachsans[0].Vung1.ThanhPho1.TenTP;
                }
                if (vung != null && vung != 0)
                {
                    ViewBag.Vung = khachsans[0].Vung1.TenVung;
                }
            }
            switch (hdgia)
            {
                case 1:
                    switch (hdsao)
                    {
                        case 1:
                            khachsans = khachsans.OrderByDescending(ks => ks.Phongs.Min(p => p.DonGia)).ThenByDescending(ks => ks.Loai).ToList();
                            break;
                        case 2:
                            khachsans = khachsans.OrderByDescending(ks => ks.Phongs.Min(p => p.DonGia)).ThenBy(ks => ks.Loai).ToList();
                            break;
                        default:
                            khachsans = khachsans.OrderByDescending(ks => ks.Phongs.Min(p => p.DonGia)).ThenByDescending(ks => ks.Diem).ThenBy(ks => ks.TenKS).ToList();
                            break;
                    }
                    break;
                case 2:
                    switch (hdsao)
                    {
                        case 1:
                            khachsans = khachsans.OrderBy(ks => ks.Phongs.Min(p => p.DonGia)).ThenByDescending(ks => ks.Loai).ToList();
                            break;
                        case 2:
                            khachsans = khachsans.OrderBy(ks => ks.Phongs.Min(p => p.DonGia)).ThenBy(ks => ks.Loai).ToList();
                            break;
                        default:
                            khachsans = khachsans.OrderBy(ks => ks.Phongs.Min(p => p.DonGia)).ThenByDescending(ks => ks.Diem).ThenBy(ks => ks.TenKS).ToList();
                            break;
                    }
                    break;
                default:
                    switch (hdsao)
                    {
                        case 1:
                            khachsans = khachsans.OrderByDescending(ks => ks.Loai).ThenByDescending(ks => ks.Diem).ThenBy(ks => ks.TenKS).ToList();
                            break;
                        case 2:
                            khachsans = khachsans.OrderBy(ks => ks.Loai).ThenByDescending(ks => ks.Diem).ThenBy(ks => ks.TenKS).ToList();
                            break;
                        default:
                            khachsans = khachsans.OrderByDescending(ks => ks.Diem).ThenBy(ks => ks.TenKS).ToList();
                            break;
                    }
                    break;
            }

            ViewBag.sao = hdsao;
            ViewBag.gia = hdgia;

            var quocgias = (from qg in dataContext.QuocGias
                            select qg);
            ViewBag.quocgias = quocgias;
            //Dem so luong khach san
            var count0 = dataContext.KhachSans.Where(ks => ks.Loai == null).Count();
            ViewBag.count0 = count0;
            var count1 = dataContext.KhachSans.Where(ks => ks.Loai == 1).Count();
            ViewBag.count1 = count1;
            var count2 = dataContext.KhachSans.Where(ks => ks.Loai == 2).Count();
            ViewBag.count2 = count2;
            var count3 = dataContext.KhachSans.Where(ks => ks.Loai == 3).Count();
            ViewBag.count3 = count3;
            var count4 = dataContext.KhachSans.Where(ks => ks.Loai == 4).Count();
            ViewBag.count4 = count4;
            var count5 = dataContext.KhachSans.Where(ks => ks.Loai == 5).Count();
            ViewBag.count5 = count5;
            ////phan trang 
            //string base_URL = "TimKiem"; //quan trong, ten action cua controller
            //int total_rows = khachsans.Count();
            //ViewBag.total_rows = total_rows;
            //string URL_segment;
            //try
            //{
            //    URL_segment = Request.Url.Segments[4];
            //}
            //catch (Exception)
            //{
            //    URL_segment = null;
            //}
            //Libraries.Pagination pagination = new Libraries.Pagination(base_URL, URL_segment, total_rows);
            //string pageLinks = pagination.GetPageLinks();
            //int start = (pagination.CurPage - 1) * pagination.PerPage;
            //int offset = pagination.PerPage;
            //if (URL_segment != null)
            //    pageLinks = pageLinks.Replace(base_URL + "/", "");
            //ViewBag.pageLinks = pageLinks;
            ////phan trang
            khachsans = khachsans.Where(ks => ks.TinhTrang == "Enabled").ToList();

            return View(khachsans);
        }

        [HttpPost]
        public JsonResult TimKiemDeal(int id_ks, DateTime? tu_ngay = null, DateTime? den_ngay = null)
        {
            DateTime from_date = Convert.ToDateTime(tu_ngay);
            DateTime to_date = Convert.ToDateTime(den_ngay);
            if (tu_ngay == null)
            {
                from_date = DateTime.Now;
            }
            if (den_ngay == null)
            {
                to_date = from_date.AddDays(7);
            }
            var deals = (from d in datacontext.Deals
                         where d.NgayBatDau <= from_date && d.NgayKetThuc >= to_date && d.KhachSan == id_ks
                         select new
                         {
                             ma_deal = d.MaDeal,
                             gia = d.Gia,
                             tien_cong_them = d.TienCongThem,
                             ma_phong = d.Phong1.MaPhong,
                             ten_phong = d.Phong1.LoaiPhong,
                             so_luong_phong = d.Phong1.SoLuong
                         });
            return Json(deals);
        }
    }
}
