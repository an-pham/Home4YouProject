using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
namespace HomeForYou.Areas.back.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /back/Admin/
        HelperController helper = new HelperController();
        public ActionResult Index()
        {
            if (Session["IsLogin"] == null)
            {
                Response.Redirect("Login");
            }
            //  helper.Permission(ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            return View("Index");
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["IsLogin"] != null)
                Index();
            return View("Login");

        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (username == null || password == null)
                return View("Login");  

            // encrypted password
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            string encrypted_pass = sb.ToString();
            // encrypted password
            var dataContext = new HomeForYouDataContext();

       
            var query = from a in dataContext.NhanViens
                        where (a.Username == username) && (a.Password == encrypted_pass) && (a.TinhTrang == "Enabled")
                        select a;
            foreach (var res in query)
            {
                Session["IsLogin"] = true;
                Session["Username"] = res.Username;
                Session["Staff_ID"] = res.MaNV;
                Session["Role"] = res.ChucVu;
                Response.Redirect("Index");
            }
            ViewBag.Message = "Sai tên tài khoản hoặc mật khẩu";
            return View("Login");
        }

        public ActionResult Logout()
        {
            if (Session["IsLogin"] != null)
            {
                HttpContext.Session["IsLogin"] = null;
                HttpContext.Session["Username"] = null;
                HttpContext.Session["Staff_ID"] = null;
                HttpContext.Session["Role"] = null;
                Response.Redirect("Login");
            }
            Response.Redirect("Login");
            return View();
        }
        public ActionResult ChangePassword()
        {
            if (Session["IsLogin"] == null)
                return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(int manv, string current_password, string new_password, string confirm_password)
        {
            if (manv == 0 || Session["IsLogin"] == null)
                return View();
            List<string> errors = new List<string>();
            if (current_password == "")
                errors.Add("Mật khẩu hiện tại không được để trống.");
            if (new_password == "")
                errors.Add("Mật khẩu mới không được để trống.");
            if (new_password.Length < 5)
                errors.Add("Mật khẩu mới phải có độ dài từ 5 ký tự trở lên.");
            if (new_password != confirm_password)
                errors.Add("Xác nhận mật khẩu chưa khớp với mật khẩu mới.");
            var dataContext = new HomeForYouDataContext();
            string encrypted_password = helper.Encrypted_Password(current_password);
            var nhanvien = (from s in dataContext.NhanViens
                            where s.MaNV == manv
                            select s).Single();
            if (nhanvien == null)
                errors.Add("Tài khoản này không tồn tại.");
            if (current_password != "" && nhanvien != null && nhanvien.Password != encrypted_password)
                errors.Add("Mật khẩu hiện tại không đúng.");
            if (errors.Count() > 0)
            {
                ViewBag.Errors = helper.Print_Error(errors);
            }
            else
            {
                encrypted_password = helper.Encrypted_Password(new_password);
                nhanvien.Password = encrypted_password;
                dataContext.SubmitChanges();
                ViewBag.Success = "Bạn đã đổi mật khẩu thành công";
            }
            return View();
        }
    }
}
