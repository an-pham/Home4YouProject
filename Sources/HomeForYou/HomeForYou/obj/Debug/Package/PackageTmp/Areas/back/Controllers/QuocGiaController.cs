using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace HomeForYou.Areas.back.Controllers
{

    public class QuocGiaController : Controller
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
        // GET: /back/QuocGia/
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("DanhSach");
        }

        public ActionResult DanhSachQuocGia(string title)
        {
            var dataContext = new HomeForYouDataContext();
            if (title != null)
            {
                List<QuocGia> quocgia = dataContext.QuocGias.Where(qgia => qgia.TenQG.Contains(title) == true).ToList();
                return View("DanhSachQuocGia", quocgia);
            }
            List<QuocGia> qg = dataContext.QuocGias.ToList();

            // kiem tra delete, edit
            foreach (var d in qg)
            {
                if (khong_xoa_sua(d.MaQG))
                {
                    d.Xoa = false;
                    d.Sua = false;
                }
            }
            return View("DanhSachQuocGia", qg);
        }

        public ActionResult ThemQuocGia()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ThemQuocGia(string tenquocgia, string vung, HttpPostedFileBase hinhdaidien)
        {
            // manage role ko quan tam
            if (!is_login)
                return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            QuocGia quocgia = new QuocGia();
            if (tenquocgia == "" || vung==""||vung==null||tenquocgia==null)
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                ViewBag.Success = 0;
                return View();
            }
            quocgia.TenQG = tenquocgia;
            quocgia.Vung = vung;
            //xu ly hinh anh
            string fileName = Path.GetFileName(hinhdaidien.FileName);
            var path = Path.Combine(Server.MapPath("~/Content/img"), fileName);
            hinhdaidien.SaveAs(path);
            quocgia.HinhDaiDien = fileName;
            //ket thuc xu ly hinh
            quocgia.TinhTrang = "Enabled";
            quocgia.Xoa = true;
            quocgia.Sua = true;
            dataContext.QuocGias.InsertOnSubmit(quocgia);
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn đã thêm quốc gia " + tenquocgia + " thành công";
            ViewBag.Success = 1;
            return View();
        }

        [HttpGet]
        public ActionResult SuaQuocGia(int maqg)
        {
            // manage role ko quan tam
            if (!is_login)
              return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            
            var quocgias = (from s in dataContext.QuocGias
                            where s.MaQG == maqg
                            select s).Single();
            return View(quocgias);
        }

        [HttpPost]
        public ActionResult SuaQuocGia(int maqg, string tenquocgia, string vung, string tinhtrang, HttpPostedFileBase hinhdaidien)
        {
            // manage role ko quan tam
            if (!is_login)
               return RedirectToAction("Login", "Admin");
            if (!helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString(), role))
                            return View("../Helper/Access_Denied");
            // manage role ko quan tam
            var dataContext = new HomeForYouDataContext();
            var quocgia = (from s in dataContext.QuocGias
                           where s.MaQG == maqg
                           select s).Single();
            if (tenquocgia == null || tenquocgia == "" || vung == null || vung == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View(quocgia);
            }
            if (hinhdaidien != null)
            {
                //Xoa hinh truoc do
                char DirSeparator = System.IO.Path.DirectorySeparatorChar;
                string filepath = "Content/img" + DirSeparator + quocgia.HinhDaiDien;
                System.IO.File.Delete(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + filepath));
                //ket thuc xoa hinh
                //upload hinh anh
                string fileName = "";
                if (hinhdaidien.ContentLength > 0)
                {
                    fileName = Path.GetFileName(hinhdaidien.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                    hinhdaidien.SaveAs(path);
                    quocgia.HinhDaiDien = hinhdaidien.FileName;
                }
                //ket thuc upload hinh
            }
            
            if (tenquocgia!="")
                quocgia.TenQG = tenquocgia;
            if (vung!="")
                quocgia.Vung = vung;
            quocgia.TinhTrang = tinhtrang;
            dataContext.SubmitChanges();
            ViewBag.Message = "Bạn cập nhật quốc gia " + tenquocgia + " thành công";
            return View(quocgia);
        }

        public bool khong_xoa_sua(int ID)
        {
            var dataContext = new HomeForYouDataContext();
            var ct = (from c in dataContext.ChiTietDatDeals
                      where c.Deal.KhachSan1.Vung1.ThanhPho1.QuocGia1.MaQG == ID
                      select c).Count();
            if (ct == 0)
            {
                return false; //cho phep xoa
            }
            return true;    //khong cho xoa
        }
        
        public void XoaQuocGia(int MaQG)
        {
            var dataContext = new HomeForYouDataContext();
            var quocgia = (from c in dataContext.QuocGias
                           where c.MaQG == MaQG
                           select c).Single();
            quocgia.TinhTrang = "Deleted";
            dataContext.SubmitChanges();
        }
       
    }
}
