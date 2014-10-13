// ********************************************************************
// Document     : HelperController.cs
// Version      : 1.0
// Category     : Libraries
// Description  : This is a helper class for managing roles in ASP.NET MVC
// Author       : Cuong Vu (Mido)
// Date         : 13/11/2012
// ********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HomeForYou.Areas.back.Controllers
{
    public class HelperController : Controller
    {
        //
        // GET: /back/Helper/
        string role;
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
          //  role = ctx.HttpContext.Session["Role"].ToString();
            if (ctx.HttpContext.Session["Role"] != null)
            {
                role = ControllerContext.HttpContext.Session["Role"].ToString();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public bool Permission(string controller, string action, string role)
        {
            if (controller == "" || action == "")
                Response.Redirect("Access_Denied");
            controller = controller.ToLower();
            action = action.ToLower();
          //  string role = (string)Session["Role"];
            switch (role)
            {
                case "admin":
                   return Permission_Admin(controller, action);
                case "manager":
                   return Permission_Manager(controller, action);
                case "staff":
                   return Permission_Staff(controller, action);
                default:
                    return false;
            }
        }

        public bool Permission_Admin(string controller, string action)
        {
            switch (controller)
            {
                case "admin": // xet nhung action trong Controller home
                    List<string> home = new List<string>() { "all" };
                    foreach (var v in home)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "nhanvien": // xet nhung action trong Controller nhanvien
                    List<string> nhanvien = new List<string>() { "all" };
                    foreach (var v in nhanvien)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                //case "customer": // xet nhung action trong Controller khachhang
                //    List<string> customer = new List<string>() { "all" };
                //    foreach (var v in customer)
                //    {
                //        if (v == "all")
                //            return true;
                //        if (v == action)
                //            return true;
                //    }
                //    break;
                case "khachhang": // xet nhung action trong Controller khachhang
                    List<string> khachhang = new List<string>() { "all" };
                    foreach (var v in khachhang)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public bool Permission_Manager(string controller, string action)
        {
            switch (controller)
            {
                case "home": // xet nhung action trong Controller home
                    List<string> home = new List<string>() { "home", "test" };
                    foreach (var v in home)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "khachhang": // xet nhung action trong Controller home
                    List<string> khachhang = new List<string>() {"all"};
                    foreach (var v in khachhang)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "khachsan": // xet nhung action trong Controller home
                    List<string> khachsan = new List<string>() { "all" };
                    foreach (var v in khachsan)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "dealks": // xet nhung action trong Controller home
                    List<string> dealks = new List<string>() { "all" };
                    foreach (var v in dealks)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "khuyenmai": // xet nhung action trong Controller home
                    List<string> khuyenmai = new List<string>() { "all" };
                    foreach (var v in khuyenmai)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "quocgia": // xet nhung action trong Controller home
                    List<string> quocgia = new List<string>() { "all" };
                    foreach (var v in quocgia)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "thanhpho": // xet nhung action trong Controller home
                    List<string> thanhpho = new List<string>() { "all" };
                    foreach (var v in thanhpho)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "vung": // xet nhung action trong Controller home
                    List<string> vung = new List<string>() { "all" };
                    foreach (var v in vung)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "baocao": // xet nhung action trong Controller home
                    List<string> baocao = new List<string>() { "all" };
                    foreach (var v in baocao)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "nhacungcap": // xet nhung action trong Controller home
                    List<string> ncc = new List<string>() { "all" };
                    foreach (var v in ncc)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                case "nhanvien": // xet nhung action trong Controller nhanvien
                    List<string> nhanvien = new List<string>() { "thongtincanhan" };
                    foreach (var v in nhanvien)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public bool Permission_Staff(string controller, string action)
        {
            switch (controller)
            {
                case "nhanvien": // xet nhung action trong Controller nhanvien
                    List<string> nhanvien = new List<string>() { "thongtincanhan" };
                    foreach (var v in nhanvien)
                    {
                        if (v == "all")
                            return true;
                        if (v == action)
                            return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
        public ActionResult Access_Denied()
        {
            return View("Access_denied");
        }

        public string Encrypted_Password(string password)
        {
            // encrypted password
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
             string encrypted_pass = sb.ToString();
             return encrypted_pass;
        }
		public  string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }
        public  string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public string Print_Error(List<string> messages)
        {
            string html = "";
            html = html + "<div class='alert alert-error' style='margin: 0px'>";
            html = html + "<button type = 'button' class = 'close' data-dismiss = 'alert'>×</button>";
            html = html + "<h4>Lỗi!</h4>";
            foreach (var m in messages)
            {
                html = html + m + "<br/>";
            }
            html = html + "</div>";
            return html;
        }
    }
}
