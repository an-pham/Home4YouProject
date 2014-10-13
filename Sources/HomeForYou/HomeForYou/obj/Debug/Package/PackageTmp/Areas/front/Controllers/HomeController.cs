using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeForYou.Areas.front.Controllers
{
	 public class DiemQuocGia
    {
        public QuocGia quocgia;
        public float diem;
    }
    public class KhachSanUaChuong
    {
        public string maKS;
        public string tenKS;
        public string tenQG;
        public string moTaDayDu;
        public string moTaNganGon;
        public string slug;
        public int loai;
        public int diem;
        public List<string> loaiPhong;
        public List<int> soPhong;
        public List<string> images;
    }
    public class kstemp
    {
        public int KhachSan;
        public float GiaReNhat;
    }
    public class HomeController : Controller
    {
        //
        // GET: /front/Home/

	public float TinhDiemQuocGia(QuocGia _qg)
        {
            var dataContext = new HomeForYouDataContext();

            float diem = 0;
            List<KhachSan> khachsans = (from qg in dataContext.QuocGias
                    join tp in dataContext.ThanhPhos on qg.MaQG equals tp.QuocGia
                    join v in dataContext.Vungs on tp.MaTP equals v.ThanhPho
                    join ks in dataContext.KhachSans on v.MaVung equals ks.Vung
                    where qg.MaQG == _qg.MaQG
                    select ks).ToList();
            diem = (from ks in khachsans
                    select (float)ks.Diem).Sum();
            return diem;
        }
        public ActionResult Index()
        {
            var dataContext = new HomeForYouDataContext();
           	List<DiemQuocGia> diemquocgias = (from qg in dataContext.QuocGias
                                              select new DiemQuocGia { diem = TinhDiemQuocGia(qg), quocgia = qg }).ToList();

			var quocgia = (from dqg in diemquocgias
                           orderby dqg.diem descending
                           select dqg.quocgia).Take(5);
            var khachsan = (from ks_an in dataContext.KhachSans
                            orderby ks_an.Diem descending, ks_an.Loai descending
                            select ks_an);
            List<kstemp> phong = (from p in dataContext.Phongs
                                  where p.DonGia > 0
                                  group p by p.KhachSan into g
                                  select new kstemp { KhachSan = (int)g.Key, GiaReNhat = (float)g.Min(p => p.DonGia) }).ToList();
            ViewBag.phong = phong;
            ViewBag.khachsan = khachsan;
            //Nhon

            //Start Lấy danh sách khách sạn
            List<KhachSanUaChuong> ks = new List<KhachSanUaChuong>();
            //Sắp xếp theo 2 tiêu chí
            //Tiêu chí 1: điểm đánh giá
            //Tiêu chí 2: loại khách sạn
            #region LayDSKhachSan


            List<KhachSanUaChuong> dsks = (from KhachSan in dataContext.KhachSans
                                           from Vung in dataContext.Vungs
                                           from ThanhPho in dataContext.ThanhPhos
                                           from QuocGia in dataContext.QuocGias
                                           where KhachSan.Vung == Vung.MaVung
                                           where Vung.ThanhPho == ThanhPho.MaTP
                                           where ThanhPho.QuocGia == QuocGia.MaQG
                                           select new KhachSanUaChuong
                                           {
                                               maKS = KhachSan.MaKS.ToString(),
                                               tenKS = KhachSan.TenKS,
                                               slug = KhachSan.Slug,
                                               tenQG = QuocGia.TenQG,
                                               moTaDayDu = KhachSan.MoTa,
                                               moTaNganGon = KhachSan.MoTa.Substring(0, 100) + "...",
                                               loai = (int)KhachSan.Loai,
                                               diem = (int)KhachSan.Diem
                                           }).OrderByDescending(x => x.diem).ThenByDescending(x => x.loai).Take(5).ToList();
            #endregion
            //End Lấy danh sách khách sạn

            //Start Lấy danh sách hình, danh sách phòng khách sạn
            #region LayDSHinh_DSPhong
            foreach (KhachSanUaChuong tungks in dsks)
            {
                //Start Lấy danh sách hình
                List<string> listImg = (from HinhKhachSan in dataContext.HinhKhachSans
                                        where HinhKhachSan.MaKS == int.Parse(tungks.maKS)
                                        select HinhKhachSan.TenHinh).ToList();
                tungks.images = listImg;
                //End Lấy danh sách hình

                //Start Lấy danh sách loại phòng
                List<string> listLoaiPhong = (from Phong in dataContext.Phongs
                                              where Phong.KhachSan == int.Parse(tungks.maKS)
                                              select Phong.LoaiPhong).Distinct().ToList();
                tungks.loaiPhong = listLoaiPhong;
                //End Lấy danh sách loại phòng


                //Start Lấy số phòng tương ứng với loại
                List<int> listSoPhong = new List<int>();
                foreach (string loaiPhong in listLoaiPhong)
                {
                    int count = (from Phong in dataContext.Phongs
                                 where Phong.KhachSan == int.Parse(tungks.maKS)
                                 where Phong.LoaiPhong == loaiPhong
                                 select Phong.SoLuong).Single().Value;
                    listSoPhong.Add(count);
                }
                tungks.soPhong = listSoPhong;
                //End Lấy số phòng tương ứng với loại
            }
            #endregion
            //End Lấy danh sách hình, danh sách phòng khách sạn

            //Start Lấy danh sách các khuyến mãi
            #region LayDSKhuyenMai
            List<KhuyenMai> dskm = new List<KhuyenMai>();
            dskm = (from KhuyenMai in dataContext.KhuyenMais
                    where KhuyenMai.TinhTrang == "Enabled"
                    select KhuyenMai).ToList();
            List<List<KhuyenMai>> groupKM = new List<List<KhuyenMai>>();
            groupKM = TaoNhomKM(2, dskm);
            #endregion
            //End Lấy danh sách các khuyến mãi
            ViewBag.dsks = dsks;
            ViewBag.groupKM = groupKM;
            //end nhon
            return View(quocgia);
            //ViewBag.QuocGia = quocgias;
        }

        private List<List<KhuyenMai>> TaoNhomKM(int soLuong1Nhom, List<KhuyenMai> dskm)
        {
            List<List<KhuyenMai>> nhomKM = new List<List<KhuyenMai>>();
            List<KhuyenMai> dsKMTemp = new List<KhuyenMai>();
            KhuyenMai KMTemp = new KhuyenMai();
            KMTemp = null;
            for (int k = 0; k < soLuong1Nhom; k++)
                dsKMTemp.Add(KMTemp);
            int i = 0;
            bool flag = false;
            while (true)
            {
                dsKMTemp = new List<KhuyenMai>();
                for (int k = 0; k < soLuong1Nhom; k++)
                    dsKMTemp.Add(KMTemp);
                for (int j = 0; j < soLuong1Nhom; j++)
                {
                    dsKMTemp[j] = dskm[i];
                    i++;
                    if (i == dskm.Count)
                    {
                        flag = true;
                        break;
                    }
                }
                nhomKM.Add(dsKMTemp);
                //MakeNull(dsKMTemp);
                if (flag == true)
                    break;
            }
            return nhomKM;
        }

        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}
