using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;

using System.Text;
using System.Net;
using System.Net.Mail;

using HomeForYou.com.paypal.sandbox.www;

namespace HomeForYou.Areas.front.Controllers
{
    public struct PayPalReturn
    {
        public bool IsSucess;
        public string ErrorMessage;
        public string TransactionID;
        public object ObjectValue;
    }

    public struct CTDatDeal
    {
        public int maDeal{get;set;}
        public int soPhong { get; set; }
        public string ngayNhan { get; set; }
        public string ngayTra { get; set; }
        public string tenKS { get; set; }
        public string tongGiaPhong { get; set; }
        public string hinhAnh { get; set; }
        public string diaDiem { get; set; }
        public string donGia { get; set; }
        public string tenLoaiPhong { get; set; }
		public string giamGia { get; set; }
    }
    
    public class ThanhToanController : Controller
    {
        //
        // GET: /front/ThanhToan/
        public static CTDatDeal temp;
		HelperController helper = new HelperController();
        public ActionResult GuiThongTin(int madeal, int sophong, string ngaynhan, string ngaytra, string error = "")
        {
            var dataContext = new HomeForYouDataContext();
            ngaynhan = ngaynhan.Replace("'", "");
            ngaytra = ngaytra.Replace("'", "");
            DateTime ngayden;
            DateTime ngayve;
            DateTime.TryParseExact(ngaynhan, "MM/dd/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ngayden);
            DateTime.TryParseExact(ngaytra, "MM/dd/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ngayve);
            int sodem = (ngayve-ngayden).Days;
            // Start thông tin đặt phòng của khách hàng
            var maks = (from s in dataContext.Deals
                        where s.MaDeal == madeal
                        select s).Single().KhachSan;
            var hinh = (from h in dataContext.HinhKhachSans
                        where h.MaKS == (int)maks
                        select h.TenHinh).ToList();
            var total = (from tc in dataContext.Deals
                         where tc.MaDeal == madeal
                         select new { Total = sophong * tc.Gia * sodem}).Single();
            
            var dongia = (from tc in dataContext.Deals
                          where tc.MaDeal == madeal
                          select new { DonGia = tc.Gia}).Single();

            var chitiet = (from ct in dataContext.Deals
                           where ct.MaDeal == madeal
                           select ct).Single();
            temp = new CTDatDeal();
            temp.hinhAnh = hinh.FirstOrDefault();
            temp.soPhong = sophong;            
            temp.tenKS = chitiet.KhachSan1.TenKS;
            temp.diaDiem = chitiet.KhachSan1.DiaChi + ", " + chitiet.KhachSan1.Vung1.TenVung + ", " + chitiet.KhachSan1.Vung1.ThanhPho1.TenTP + ", " + chitiet.KhachSan1.Vung1.ThanhPho1.QuocGia1.TenQG;
            temp.ngayNhan = ngaynhan;
            temp.ngayTra = ngaytra;    
            temp.donGia = dongia.DonGia.ToString();
            temp.tongGiaPhong = total.Total.ToString();
            temp.tenLoaiPhong = chitiet.Phong1.LoaiPhong;
            temp.maDeal = madeal;

			temp.giamGia = "0%";
            ViewBag.CTDatDeal = temp;
            ViewBag.Error = error;
            //End thông tin đặt phòng của khách hàng 
            return View();
        }
        //Start GuiThongTin
        [HttpPost]
        public ActionResult EndStep2
        (string optionRadios_tt, string client_code, string firstname, string lastname,
        string email, string email_confirm, string cmnd, string phone_number, string address,
        string credit_card,  int _madeal, int _sophong, string _ngaynhan, string _ngaytra)
        {
            var dataContext = new HomeForYouDataContext();
            //Start Trường hợp 1: Đã có thẻ khách hàng
            #region da_co_the_khach_hang
            if (optionRadios_tt == "option1")
            {
                //if (IsClientCode(client_code))
                var kh = (from KhachHang in dataContext.KhachHangs where KhachHang.SoTheKH == client_code select KhachHang).FirstOrDefault();
                //Start Tạo chi tiết mới
                ChiTietDatDeal ctDeal = new ChiTietDatDeal();
                ctDeal.MaDeal = _madeal;
                ctDeal.MaKH = kh.ID;
                ctDeal.SoPhong = _sophong;
                ctDeal.NgayNhan = DateTime.Parse(_ngaynhan);
                ctDeal.NgayTra = DateTime.Parse(_ngaytra);
                ctDeal.TinhTrang = "Unpaid";

				//Giảm giá với khách hàng thân thiết theo điểm tích lũy
                //1 lần đặt + 1 điểm
                //>=10 điểm giảm 5%
                //>=20 điểm giảm 10%
                if (kh.TichLuy >= 20)
                {
                    double t = Double.Parse(temp.tongGiaPhong) - Double.Parse(temp.tongGiaPhong) * 0.1;
                    temp.tongGiaPhong = t.ToString();
                    temp.giamGia = "10%";
                }
                else
                {
                    if (kh.TichLuy >= 10)
                    {
                        double t = Double.Parse(temp.tongGiaPhong) - Double.Parse(temp.tongGiaPhong) * 0.05;
                        temp.tongGiaPhong = t.ToString();
                        temp.giamGia = "5%";
                    }
                    else
                        temp.giamGia = "0%";
                }
                dataContext.ChiTietDatDeals.InsertOnSubmit(ctDeal);
                try
                {
                    dataContext.SubmitChanges();
                    var ct = dataContext.ChiTietDatDeals.OrderByDescending(x => x.MaCT).Take(1).Single();
                    return RedirectToAction("NhapDiaChi", new { client_code = client_code, ctdeal = ct.MaCT});
                }
                catch
                {
                    return RedirectToAction("GuiThongTin", new{error = "Lỗi truy xuất dữ liệu!"});
                }
                //End Tạo chi tiết mới
            }
            #endregion
            //End Trường hợp 1: Đã có thẻ khách hàng
            //Start Trường hợp 2: Chưa có thẻ khách hàng
            #region chua_co_the_khach_hang
            else
            {
                //Kiểm tra thông tin đầu vào
                //Nếu không đủ thông tin thì báo lỗi
                if (firstname == "" || lastname == "" || email == "" || email_confirm == "" || cmnd == "" || firstname == null || lastname == null || email == null || email_confirm == null || cmnd == null)
                {
                    ViewBag.Error = "Thông tin nhập vào chưa đầy đủ!";
                    //return View("GuiThongTin", new { madeal = 1, sophong = 2, ngaynhan = "", ngaytra = "", error = "Thông tin nhập vào chưa đầy đủ!" });
                    return RedirectToAction("GuiThongTin", new { madeal = 1, sophong = 2, ngaynhan = "", ngaytra = "", error = "Thông tin nhập vào chưa đầy đủ!" });
                }
                //Nếu đủ thông tin thì tiến hành tạo thẻ mới
                else
                {
					//if (!helper.Valid_Email(email))
                    //{
                    //    ViewBag.Error = "Email không hợp lệ!";
                    //    return RedirectToAction("GuiThongTin", new { madeal = 1, sophong = 2, ngaynhan = "", ngaytra = "", error = "Email lặp lại không hợp lệ!" });
                    //}
                    //Kiểm tra email
                    if (email != email_confirm)
                    {
                        ViewBag.Error = "Email lặp lại không trùng!";
                        return RedirectToAction("GuiThongTin", new { madeal = 1, sophong = 2, ngaynhan = "", ngaytra = "", error = "Email lặp lại không trùng!" });
                        
                    }
                    //Start Tạo mã thẻ mới
                    else
                    {
                        string new_client_code = RandomString(8);
                        //Nếu code đã tồn tại thì tạo code mới
                        while (IsClientCode(new_client_code))
                        {
                            new_client_code = RandomString(8);
                        }
                        KhachHang kh = new KhachHang();
                        kh.SoTheKH = new_client_code;
                        kh.Ho = lastname;
                        kh.Ten = firstname;
                        kh.Email = email;
                        kh.CMND = cmnd;
                        kh.SoDT = phone_number;
                        kh.DiaChi = address;
                        kh.TinhTrang = "Enabled";
                        kh.Xoa = true;
                        kh.Sua = true;
                        if (credit_card != "")
                            kh.TheTinDung = credit_card;
                        dataContext.KhachHangs.InsertOnSubmit(kh);
                        try
                        {
                            dataContext.SubmitChanges();
                            var kh1 = dataContext.KhachHangs.OrderByDescending(x => x.ID).Take(1).Single();
                            //Start Tạo chi tiết mới
                            ChiTietDatDeal ctDeal = new ChiTietDatDeal();
                            ctDeal.MaDeal = _madeal;
                            ctDeal.MaKH = kh1.ID;
                            ctDeal.SoPhong = _sophong;
                            ctDeal.NgayNhan = DateTime.Parse(_ngaynhan);
                            ctDeal.NgayTra = DateTime.Parse(_ngaytra);
                            ctDeal.TinhTrang = "Unpaid";

                            dataContext.ChiTietDatDeals.InsertOnSubmit(ctDeal);
                            try
                            {
                                dataContext.SubmitChanges();
                                var ct = dataContext.ChiTietDatDeals.OrderByDescending(x => x.MaCT).Take(1).Single();
                                return RedirectToAction("NhapDiaChi", new { client_code = new_client_code, ctdeal = ct.MaCT});
                            }
                            catch
                            {
                                return RedirectToAction("GuiThongTin", new { error = "Lỗi truy xuất dữ liệu!" });
                            }
                        }
                        catch
                        {
                            ViewBag.Error = "Lỗi truy xuất dữ liệu!";
                            return View("GuiThongTin");
                        }
                    }
                    //End Tạo mã thẻ mới
                }
            }
            #endregion
            //End Trường hợp 2: Chưa có thẻ khách hàng
        }
        [HttpPost]
        public bool IsClientCode(string code)
        {
            var context = new HomeForYouDataContext();
            return (from KhachHang in context.KhachHangs select KhachHang).Any(x => x.SoTheKH.Equals(code)); ;
        }

        [HttpPost] // can be HttpGet
        public JsonResult Test(string code)
        {
            var context = new HomeForYouDataContext();
            bool isValid = (from kh in context.KhachHangs select kh).Any(x => x.SoTheKH.Equals(code)) ;
            return Json(isValid);
        }
        public string RandomString(int len)
        {
            string str = "";
            Random ran = new Random();
            for (int i = 0; i < len; i++)
            {
                str += ran.Next(0, 9);
            }
            return str;
        }
        //End Nhơn
        public ActionResult NhapDiaChi(int ctdeal, string client_code, string slug = null)
        {
            var dataContext = new HomeForYouDataContext();            
            ViewBag.CTDatDeal = temp;

            //End thông tin đặt phòng của khách hàng 
            // Start thông tin địa chỉ của KH
            var khachhang = (from kh in dataContext.KhachHangs
                             where kh.SoTheKH == client_code
                             select kh).Single();
            ViewBag.khachhang = khachhang;
            ViewBag.maCT = ctdeal;
            // End thông tin địa chỉ của KH

            // Start phương thức thanh toán
            //List<PhuongThucThanhToan> thanhtoan = dataContext.PhuongThucThanhToans.ToList();
            //ViewBag.thanhtoan = new SelectList(thanhtoan, "MaPT", "TenPT");

            var listPT = (from PhuongThucThanhToan in dataContext.PhuongThucThanhToans where PhuongThucThanhToan.TinhTrang == "Enabled" select PhuongThucThanhToan).ToList();
            ViewBag.listPT = listPT;
            // End phương thức thanh toán
            return View();
           
        }

        public static KhachHang khTemp;
        [HttpPost]
        public ActionResult EndStep3(string email, int _tongtien, int _maCT, int phuongthuc, string _sothe, string _socvc, string expMonth, string expYear, string buyerLastName, string buyerFirstName, string buyerAddress, string typeofcard, string clientcode)
        {
           
            //// Kiem tra ma the nhap vao 
            //if (_sothe == "" || _socvc == "")
            //{
            //    var dataContext = new HomeForYouDataContext();
            //    var khachhang = (from ChiTietDatDeal in dataContext.ChiTietDatDeals
            //                     from KhachHang in dataContext.KhachHangs
            //                     where ChiTietDatDeal.MaKH == KhachHang.ID
            //                     where ChiTietDatDeal.MaCT == _maCT
            //                     select KhachHang.SoTheKH).Single();

            //    ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
            //    return View("NhapDiaChi", _maCT, khachhang);
            //}

            // End kiem tra thong tin the
            //Nếu chưa chọn phương thức
            if (phuongthuc == 0)
            {
                ViewBag.Error = "Chưa chọn phương thức thanh toán!";
                return View("NhapDiaChi");
            }
            else
            {
                //Nếu có chọn phương thức
                var dataContext = new HomeForYouDataContext();

                // Start thông tin địa chỉ của KH
                var khachhang = (from ChiTietDatDeal in dataContext.ChiTietDatDeals
                                 from KhachHang in dataContext.KhachHangs
                                 where ChiTietDatDeal.MaKH == KhachHang.ID
                                 where ChiTietDatDeal.MaCT == _maCT
                                 select KhachHang).Single();
                ViewBag.khachhang = khachhang;
                khTemp = khachhang;
                // End thông tin địa chỉ của KH
                ViewBag.CTDatDeal = temp;
                ViewBag.maCT = _maCT;
                // Start Tạo hóa đơn và sửa tình trạng chitietdatdeals khi thanh toán bằng phương thức Thẻ tín dụng
                switch(phuongthuc)
                {
                    case 1:
                    {
                        //Start tạo hóa đơn
                        HoaDon hoadon = new HoaDon();
                        hoadon.NgayThanhToan = DateTime.Now;
                        hoadon.TongTien = _tongtien;
                        hoadon.PhuongThuc = phuongthuc;
                        hoadon.CTDeal = _maCT;
                        hoadon.TinhTrang = "Enabled";
                        //End tạo hóa đơn

                        //Start cập nhật chitietdatdeal
                        var chitietdatdeal = (from c in dataContext.ChiTietDatDeals
                                              where c.MaCT == _maCT
                                              select c).Single();
                        chitietdatdeal.TinhTrang = "Unpaid";
                        //End cập nhật chitietdatdeal

                        //Cập nhật điểm tích lũy
                        khachhang.TichLuy++;
                        dataContext.HoaDons.InsertOnSubmit(hoadon);
                        dataContext.SubmitChanges();

                        GuiMail(email, hoadon);
                    }
                    break;
                    case 2:
                    {
                        PayPalReturn rv;
                        rv = Pay(Convert.ToString(_tongtien/20000), buyerLastName, buyerFirstName, buyerAddress, typeofcard, _sothe, _socvc, expMonth, expYear); // 20000 tỷ giá USD
                        if (rv.IsSucess)
                        {
                            //Start tạo hóa đơn
                            HoaDon hoadon = new HoaDon();
                            hoadon.NgayThanhToan = DateTime.Now;
                            hoadon.TongTien = _tongtien;
                            hoadon.PhuongThuc = phuongthuc;
                            hoadon.CTDeal = _maCT;
                            hoadon.TinhTrang = "Enabled";

                            //End tạo hóa đơn

                            //Start cập nhật chitietdatdeal
                            var chitietdatdeal = (from c in dataContext.ChiTietDatDeals
                                                  where c.MaCT == _maCT
                                                  select c).Single();
                            chitietdatdeal.TinhTrang = "Paid";
                            //End cập nhật chitietdatdeal

                            //Cập nhật điểm tích lũy
                            khachhang.TichLuy++;
                            dataContext.HoaDons.InsertOnSubmit(hoadon);
                            dataContext.SubmitChanges();

                            GuiMail(email, hoadon);
                        }
                        else
                        {
                            return RedirectToAction("NhapDiaChi", new { ctdeal = _maCT, client_code = clientcode});
                        }
                    }
                    break;
                    default:
                    break;
                }
                return RedirectToAction("HoanTat", new { maCT = _maCT, phuongthuc = phuongthuc });
                // End Tạo hóa đơn và sửa tình trạng chitietdatdeals khi thanh toán bằng phương thức Thẻ tín dụng
            }
        }

        public ActionResult HoanTat(int maCT, int phuongthuc)
        {
            var dataContext = new HomeForYouDataContext();
            //Nếu phương thức là thẻ tín dụng (phuongthuc = 2)
            //Select ra hóa đơn
            if (phuongthuc == 2)
            {
                var hoadon = (from HoaDon in dataContext.HoaDons where HoaDon.CTDeal == maCT select HoaDon).Single();
                ViewBag.HoaDon = hoadon;
            }
            var tenPT = (from PhuongThucThanhToan in dataContext.PhuongThucThanhToans where PhuongThucThanhToan.MaPT == phuongthuc select PhuongThucThanhToan.TenPT).Single();
            ViewBag.maCT = maCT;
            ViewBag.tenPT = tenPT;
            ViewBag.CTDatDeal = temp;
            ViewBag.khachhang = khTemp;
            return View();          
        }
        public JsonResult Valid_Email(string email)
        {
            //string mail_regex = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
            string mail_regex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            bool isValid = Regex.IsMatch(email, mail_regex);
            return Json(isValid);
        }

        // Thanh toán Paypal
        public PayPalReturn Pay(string paymentAmount, string buyerLastName, string buyerFirstName, string buyerAddress, string creditCardType, string creditCardNumber, string CVV2, string expMonth, string expYear)
        {
            //PayPal Return Structure
            PayPalReturn rv = new PayPalReturn();
            rv.IsSucess = false;

            DoDirectPaymentRequestDetailsType requestDetails = new DoDirectPaymentRequestDetailsType();
            requestDetails.CreditCard = new CreditCardDetailsType();
            requestDetails.CreditCard.CardOwner = new PayerInfoType();
            requestDetails.CreditCard.CardOwner.Address = new AddressType();
            requestDetails.PaymentDetails = new PaymentDetailsType();
            requestDetails.PaymentDetails.OrderTotal = new BasicAmountType();
            requestDetails.CreditCard.CardOwner.PayerName = new PersonNameType();

            //Request
            requestDetails.PaymentAction = PaymentActionCodeType.Sale;
            requestDetails.IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            //Payment
            requestDetails.PaymentDetails.OrderTotal.currencyID = CurrencyCodeType.USD;
            requestDetails.PaymentDetails.OrderTotal.Value = paymentAmount;

            //Credit card
            requestDetails.CreditCard.CreditCardNumber = creditCardNumber;
            requestDetails.CreditCard.CreditCardType = (CreditCardTypeType)Enum.Parse(typeof(CreditCardTypeType), creditCardType, true);
            requestDetails.CreditCard.ExpMonth = Convert.ToInt32(expMonth);
            requestDetails.CreditCard.ExpYear = Convert.ToInt32(expYear);
            requestDetails.CreditCard.CVV2 = CVV2;
            requestDetails.CreditCard.CreditCardTypeSpecified = true;
            requestDetails.CreditCard.ExpMonthSpecified = true;
            requestDetails.CreditCard.ExpYearSpecified = true;

            //Card Owner
            requestDetails.CreditCard.CardOwner.PayerName.FirstName = buyerFirstName;
            requestDetails.CreditCard.CardOwner.PayerName.LastName = buyerLastName;
            requestDetails.CreditCard.CardOwner.Address.Street1 = buyerAddress;
            //requestDetails.CreditCard.CardOwner.Address.CityName = buyerCity;
            //requestDetails.CreditCard.CardOwner.Address.StateOrProvince = buyerStateOrProvince;
            //requestDetails.CreditCard.CardOwner.Address.CountryName = buyerCountryName;
            //requestDetails.CreditCard.CardOwner.Address.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), buyerCountryCode, true);
            //requestDetails.CreditCard.CardOwner.Address.PostalCode = buyerZipCode;
            //requestDetails.CreditCard.CardOwner.Address.CountrySpecified = true;
            //requestDetails.CreditCard.CardOwner.PayerCountry = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), buyerCountryCode, true);
            //requestDetails.CreditCard.CardOwner.PayerCountrySpecified = true;

            DoDirectPaymentReq request = new DoDirectPaymentReq();
            request.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            request.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = requestDetails;
            request.DoDirectPaymentRequest.Version = "51.0";

            //Headers
            CustomSecurityHeaderType headers = new CustomSecurityHeaderType();
            headers.Credentials = new UserIdPasswordType();
            headers.Credentials.Username = ConfigurationManager.AppSettings["PayPalAPIUsername"];
            headers.Credentials.Password = ConfigurationManager.AppSettings["PayPalAPIPassword"];
            headers.Credentials.Signature = ConfigurationManager.AppSettings["PayPalAPISignature"];

            //Client
            PayPalAPIAASoapBinding client = new PayPalAPIAASoapBinding();
            client.RequesterCredentials = headers;
            client.Timeout = 15000;
            DoDirectPaymentResponseType response = client.DoDirectPayment(request);
            if (response.Ack == AckCodeType.Success || response.Ack == AckCodeType.SuccessWithWarning)
            {
                rv.IsSucess = true;
                rv.TransactionID = response.TransactionID;
            }
            else
            {
                rv.ErrorMessage = response.Errors[0].LongMessage;
            }
            return rv;
        }

        public PayPalReturn GetTransactionDetails(string transactionID)
        {
            //PayPal Return Structure
            PayPalReturn rv = new PayPalReturn();
            rv.IsSucess = false;

            //Requests
            //TransactionID = "6XT85330WL909250J"
            GetTransactionDetailsReq request = new GetTransactionDetailsReq();
            request.GetTransactionDetailsRequest = new GetTransactionDetailsRequestType();
            request.GetTransactionDetailsRequest.TransactionID = transactionID;
            request.GetTransactionDetailsRequest.Version = "51.0";

            //Headers
            CustomSecurityHeaderType headers = new CustomSecurityHeaderType();
            headers.Credentials = new UserIdPasswordType();
            headers.Credentials.Username = ConfigurationManager.AppSettings["PayPalAPIUsername"];
            headers.Credentials.Password = ConfigurationManager.AppSettings["PayPalAPIPassword"];
            headers.Credentials.Signature = ConfigurationManager.AppSettings["PayPalAPISignature"];

            //Client
            PayPalAPISoapBinding client = new PayPalAPISoapBinding();
            client.RequesterCredentials = headers;
            client.Timeout = 15000;
            GetTransactionDetailsResponseType response = client.GetTransactionDetails(request);

            if (response.Ack == AckCodeType.Success || response.Ack == AckCodeType.SuccessWithWarning)
            {
                rv.IsSucess = true;
                rv.TransactionID = response.PaymentTransactionDetails.PaymentInfo.TransactionID;
                rv.ObjectValue = response.PaymentTransactionDetails;
            }
            else
            {
                rv.ErrorMessage = response.Errors[0].LongMessage;
            }
            return rv;
        }


        [HttpPost]
        public int GuiMail(string emailaddr, HoaDon hd)
        {
            HomeForYouDataContext datacontext = new HomeForYouDataContext();
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("home4u.somee.com@gmail.com");

            mail.To.Add(emailaddr);

            mail.Subject = "[Home4U] - Thông tin đặt phòng.";

            mail.IsBodyHtml = true;

            var thongtin = (from hoadon in datacontext.HoaDons
                            join ctdeal in datacontext.ChiTietDatDeals on hoadon.CTDeal equals ctdeal.MaCT
                            join deal in datacontext.Deals on ctdeal.MaDeal equals deal.MaDeal
                            join phong in datacontext.Phongs on deal.Phong equals phong.MaPhong
                            join khachsan in datacontext.KhachSans on phong.KhachSan equals khachsan.MaKS
                            where hoadon.MaHD == hd.MaHD
                            select new { ctdeal.KhachHang.SoTheKH, ctdeal.KhachHang.Ten, ctdeal.KhachHang.Ho, hoadon.PhuongThucThanhToan.TenPT, hoadon.TongTien, ctdeal.SoPhong, ctdeal.NgayNhan, ctdeal.NgayTra, phong.LoaiPhong, khachsan.TenKS }).First();

            string str = "Home4U - Đặt khách sạn thật dễ!";
            str += "<br />Xin chào Ông (bà) " + thongtin.Ho + " " + thongtin.Ten;
            str += "<br />Cảm ơn quý khách đã sử dụng dịch vụ tại công ty chúng tôi.";
            str += "<br />Thông tin đặt phòng của quý khách: ";
            str += "<br /> <b> Khách sạn: </b>"+ thongtin.TenKS;
            str += "<br /> <b> Loại phòng: </b>" + thongtin.LoaiPhong;
            str += "<br /> <b> Số phòng: </b>" + thongtin.SoPhong;
            str += "<br /> <b> Ngày nhận: </b>" + thongtin.NgayNhan;
            str += "<br /> <b> Ngày trả: </b>" + thongtin.NgayTra;
            str += "<br /> <b> Tổng tiền: </b>" + thongtin.TongTien;
            str += "<br /> <b> Phương thức thanh toán: </b>" + thongtin.TenPT;

            str += "<br /> <b> Mã số thẻ thành viên của ông (bà) là: </b>" + thongtin.SoTheKH;
            str += "<br /> <b> ----------------------------------------------------------------------------- </b>";
            str += "<br />Kính chúc quý khách một ngày tốt lành.";

            str += "<br />--";
            str += "<br />Home4U.";
            mail.Body = "<p>" + str + "</p>";

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";

            smtp.EnableSsl = true;

            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();

            NetworkCred.UserName = "home4u.somee.com@gmail.com";

            NetworkCred.Password = "home4you";

            smtp.UseDefaultCredentials = true;

            smtp.Credentials = NetworkCred;

            smtp.Port = 587;

            smtp.Send(mail);

            return 1;
        }
    }
}
