using Boutique.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Boutique.Patterns.Strategy;

namespace Boutique.Controllers
{
    public class CartController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> listCart = getCart();
            return View(listCart);
        }

        public List<Cart> getCart()
        {
            List<Cart> listCart = Session["Cart"] as List<Cart>;

            if (listCart == null)
            {
                listCart = new List<Cart>();
                Session["Cart"] = listCart;

            }
            ViewBag.totalPrice = TongTien();
            ViewBag.totalQuantity = totalQuantity();
            return listCart;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult addtoCart(int IdProduct, int IdColor, int IdSize, string strURL)
        {
            // Lấy ra session giỏ hàng
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            Cart product = listCart.Find(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize);
            if (product == null)
            {
                product = new Cart(IdProduct, IdColor, IdSize);
                listCart.Add(product);
            }
            else
            {
                product.Quantity++;
            }
            ViewBag.noti = "Đã thêm sản phẩm vào giỏ hàng";
            return Content(strURL);
        }
        // Tính tổng số lượng sản phẩm
        public int totalQuantity()
        {
            int itotalQuantity = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                itotalQuantity = listCart.Sum(n => n.Quantity);
            }
            return itotalQuantity;
        }
        //Tính tổng tiền sản phẩm
        private double TongTien()
        {
            double iTongTien = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                iTongTien = listCart.Sum(n => n.totalPrice);
            }
            return iTongTien;
        }
        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult deletefromCart(int IdProduct, int IdColor, int IdSize)
        {
            List<Cart> listCart = getCart();
            Cart product = listCart.SingleOrDefault(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize);
            if (product != null)
            {
                listCart.RemoveAll(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize);
                return RedirectToAction("Index");
            }
            if (listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }
        // Cập nhật giỏ hàng
        public ActionResult UpdateCart(int IdProduct, int IdColor, int IdSize, FormCollection f)
        {
            // Lấy giỏ hàng từ session
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            Cart product = listCart.SingleOrDefault(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize);
            if (product != null)
            {
                product.Quantity = int.Parse(f["quantity"].ToString());
            }
            return RedirectToAction("Index");
        }
        // Đặt hàng
        public ActionResult getPromotion()
        {
            var result = _db.Promotions.Where(m => m.end_date > DateTime.Now).ToList();
            return Json(new { Data = result, TotalItems = result.Count }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getCode(string Code)
        {
            var now = DateTime.Now;
            var item = _db.Promotions.SingleOrDefault(i => i.promotion_name.Equals(Code) && i.end_date > now);
            return Json(new { data = item }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckOut()
        {
            List<Cart> listCart = getCart();
            if (Session["Cart"] == null || listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cart = listCart;
            Customer model = (Customer)Session["Taikhoan"];
            if (model == null)
            {
                model = new Customer();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckOut(Customer model, Order orderModel, string promoCode)
        {
            Order order = new Order();
            info_Order(order, orderModel);
            order.CustomerId = CustomerId(model);
            order.TotalPrice = orderPrice(promoCode);
            _db.Orders.Add(order);
            _db.SaveChanges();
            info_Detail(order.Id);
            Session["OrderConfirmed"] = true;
            switch (orderModel.PaymentId)
            {
                case 1:
                    return Json(new { redirectToUrl = Url.Action("PaymentMomo", new { Id = order.Id }) });
                case 2:
                    return Json(new { redirectToUrl = Url.Action("Payment", new { orderId = order.Id }) });
                case 3:
                    return Json(new { redirectToUrl = Url.Action("confirmOrder", new { Id = order.Id }) });
            }
            return Json(new { something = "Có lỗi xảy ra" });
        }

        public int CustomerId(Customer model)
        {
            if (model.Id != 0)
            {
                return model.Id;
            }

            Customer kh = new Customer();
            kh.Password = "Nhan123?";
            kh.FullName = model.FullName;
            kh.Phone = model.Phone;
            kh.Email = model.Email;
            kh.Address = model.Address;
            kh.Member = false;
            _db.Customers.Add(kh);
            _db.SaveChanges();
            return kh.Id;
        }

        public double orderPrice(string promoCode)
        {
            double totalPrice = TongTien();

            DateTime now = DateTime.Now;
            Promotion codeKM = _db.Promotions.SingleOrDefault(m =>
                                  m.promotion_name == promoCode && 
                                  m.end_date > now);

            double percent = (codeKM != null) ? 
            (double)codeKM.discount_percentage :
            100;

            var CustomerBill = (now.Month == 10) ? 
                new CustomerBill(new HappyMonthStrategy()) : 
                new CustomerBill(new NormalStrategy());

            if(now.Hour == 19) CustomerBill = new CustomerBill(new HappyHourStrategy());
            return CustomerBill.LastPrice(totalPrice, percent);
        }

        public void info_Order(Order order, Order orderModel)
        {
            order.OrdTime = DateTime.Now;
            order.DeliTime = order.OrdTime.Value.AddDays(3);
            order.Status = "Chưa giao hàng";
            order.PaymentId = orderModel.PaymentId;
            order.Address = orderModel.Address;
            order.Note = orderModel.Note;
            order.TotalQuantity = totalQuantity();
        }

        public void info_Detail(int orderId)
        {
            List<Cart> listCart = getCart();
            foreach (var item in listCart)
            {
                OrderDetail detailOrd = new OrderDetail();
                detailOrd.OrderId = orderId;
                detailOrd.StockId = item.IdStock;
                detailOrd.Quantity = item.Quantity;
                detailOrd.unitPrice = item.unitPrice * item.Quantity;
                _db.OrderDetails.Add(detailOrd);
                Product product = _db.Products.SingleOrDefault(p =>
                p.Id == item.IdProduct);
                product.Sold++;
                Stock stock = _db.Stocks.SingleOrDefault(s =>
                s.ProductId == item.IdProduct
                && s.ColorId == item.IdColor
                && s.SizeId == item.IdSize);
                stock.Stock1--;
                _db.Entry(stock).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
        // Xác nhận đơn hàng
        public ActionResult confirmOrder(int? Id)
        {
            if (Session["OrderConfirmed"] == null || (bool)Session["OrderConfirmed"] == false)
            {
                return RedirectToAction("Index");
            }
            Order order = _db.Orders.SingleOrDefault(o => o.Id == Id);
            if (Id == null || order == null)
            {
                return RedirectToAction("Index");
            }
            //sendPass(order);
            Session["OrderConfirmed"] = false;
            Session["Cart"] = null;
            var ordDetail = _db.OrderDetails.ToList();
            ViewBag.ordDetail = ordDetail;
            return View(order);
        }
        // Mail
        public void sendPass(Order order)
        {
            var orderDetail = _db.OrderDetails.Where(p => p.OrderId == order.Id).ToList();
            StringBuilder htmlBuilder = new StringBuilder();
            foreach (var item in orderDetail)
            {
                htmlBuilder.AppendLine(item.Stock.Product.Name
                + "-" + item.Stock.Color.Name
                + "-" + item.Stock.Size.Name
                + " x" + item.Quantity
                + "</p>\n" + "</br>");
            }
            MailAddress fromAddress = new MailAddress("nhancmvn12@gmail.com", "Nhân Boutique");
            MailAddress toAddress = new MailAddress("nhancmvn12@gmail.com");
            const string fromPassword = "jznrdhzrgpnsxswp";
            string subject = "Đơn hàng mới #" + order.Id.ToString();
            string body = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Xác nhận đơn hàng</title>
            </head>
            <body>
                <h1>Đơn hàng mới !!! " + "</h1>" +
                "<h2>Thông tin khách hàng:</h2>" +
                "<p><strong>Tên khách hàng:</strong> " + order.Customer.FullName + "</p>" +
                "<p><strong>Email:</strong> " + order.Customer.Email + "</p>" +
                "<p><strong>Địa chỉ:</strong> " + order.Address + "</p>" +
                "<p><strong>Số điện thoại:</strong> " + order.Customer.Phone + "</p>" +
                "<h2>Thông tin đơn hàng:</h2>" +
                htmlBuilder.ToString() +
                "<p><strong>Mã đơn hàng:</strong> " + order.Id + "</p>" +
                "<p><strong>Ngày đặt hàng:</strong> " + order.OrdTime?.ToString("dd/MM/yyyy") + "</p>" +
                "<p><strong>Tình trạng đơn hàng:</strong> " + order.Status + "</p>" +
                "<p><strong>Hình thức vận chuyển:</strong> " + order.Payment.Ghichu + "</p>" +
                "<p><strong>Dự kiến giao hàng:</strong> " + order.DeliTime?.ToString("dd/MM/yyyy") + "</p>" +
                "<p><strong>Giá trị đơn hàng:</strong> " + string.Format("{0:#,0}", order.TotalPrice) + "VNĐ" + "</p>" +
                "<!-- Thêm thông tin khác về đơn hàng nếu cần -->" +
                "<p>Kiểm tra thông tin đơn hàng và giao hàng.</p>" +
                "<p>Nhân Boutique</p>" +
                "</body></html>";
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })
            {
                smtp.Send(message);
            }
        }
        //thanh toán VNPAY
        public ActionResult Payment(int orderId) // tạo yêu cầu thanh toán và chuyển hướng người dùng đến trang thanh toán
        {
            Order order = _db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                // Xử lý lỗi khi không tìm thấy đơn hàng
                return RedirectToAction("Index", "Home"); // Hoặc chuyển hướng tới trang lỗi
            }
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            DateTime expirationTime = DateTime.Now.AddDays(1);
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (order.TotalPrice * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", expirationTime.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", orderId.ToString());  //mã hóa đơn
            //pay.AddRequestData("vnp_payment", order.Payment.ToString()); //mã hóa đơn
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);
        }
        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();
                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                Order order = _db.Orders.FirstOrDefault(o => o.Id == orderId);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + order.Id.ToString() + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        var orderDetail = _db.OrderDetails.Where(o => o.OrderId == order.Id).ToList();
                        foreach (var item in orderDetail)
                        {
                            _db.OrderDetails.Remove(item);
                            _db.SaveChanges();
                        }
                        _db.Orders.Remove(order);
                        _db.SaveChanges();
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
                return RedirectToAction("confirmOrder", new { Id = order.Id });
            }
            return RedirectToAction("Index");
        }
        // Momo payment
        public ActionResult PaymentMomo(int Id)
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng từ Nhân Boutique";
            string returnUrl = "http://thanhnhanne-001-site1.atempurl.com/Cart/ConfirmPaymentClient/";
            string notifyurl = "http://thanhnhanne-001-site1.atempurl.com/Cart/ConfirmPaymentClient/"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            DateTime expirationTime = DateTime.Now.AddDays(1);
            Order order = _db.Orders.Find(Id);
            string orderId = order.Id.ToString(); //mã đơn hàng
            string amount = order.TotalPrice.ToString(); // giá trị đơn hàng
            string requestId = order.Id.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderId + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            if (rErrorCode == "0")
            {
                return RedirectToAction("confirmOrder", new { Id = int.Parse(result.orderId) });
            }
            else
            {
                return RedirectToAction("confirmOrder");
            }
        }
        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
            
        }
    }
}