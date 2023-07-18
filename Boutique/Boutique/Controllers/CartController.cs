using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Boutique.Controllers
{
    public class CartController : Controller
    {
        BoutiqueEntities _db = new BoutiqueEntities();
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
        public ActionResult addtoCart(int IdProduct,int IdColor, int IdSize, string strURL)
        {
            // Lấy ra session giỏ hàng
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            Cart product = listCart.Find(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize );
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
            // Lấy giỏ hàng từ session
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            Cart product = listCart.SingleOrDefault(n => n.IdProduct == IdProduct && n.IdColor == IdColor && n.IdSize == IdSize);
            //nếu tồn tại thì sửa số lượng
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
        public ActionResult UpdateCart(int IdProduct, int IdColor , int IdSize, FormCollection f)
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
        public ActionResult CheckOut()
        {
            List<Cart> listCart = getCart();
            if (Session["Cart"] == null || listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cart = listCart;
            Customer model = (Customer)Session["Taikhoan"];
            if(model == null)
            {
                model = new Customer();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckOut(Customer model,FormCollection f)
        {
            getCart();
            var products = _db.Products.ToList();
            var address = f["address"];
            var note = f["note"];
            var check = _db.Customers.FirstOrDefault(s => s.Email.Equals(model.Email));
            ModelState.Remove("Password");
            if (ModelState.IsValid) 
            {
                if (check == null)
                {
                    Customer customer = new Customer();
                    customer.Password = "Nhan123?";
                    customer.FullName = model.FullName;
                    customer.Phone = model.Phone;
                    customer.Email = model.Email;
                    customer.Member = false;
                    _db.Customers.Add(customer);
                    _db.SaveChanges();
                }
                Customer cus = _db.Customers.SingleOrDefault(s => s.Email.Equals(model.Email));
                if (cus.Member == false)
                {
                    cus.FullName = model.FullName;
                    cus.Phone = model.Phone;
                    _db.Entry(cus).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                Order order = new Order();
                order.CustomerId = cus.Id;
                order.OrdTime = DateTime.Now;
                order.DeliTime = order.OrdTime.Value.AddDays(3);
                order.Status = "Chưa giao hàng";
                order.Payment = false;
                order.Address = address.ToString();
                order.TotalPrice = TongTien();
                order.TotalQuantity = totalQuantity();
                order.Note = note.ToString();
                _db.Orders.Add(order);
                _db.SaveChanges();
                List<Cart> listCart = getCart();
                foreach (var item in listCart)
                {
                    OrderDetail detailOrd = new OrderDetail();
                    detailOrd.OrderId = order.Id;
                    detailOrd.StockId = item.IdStock;
                    detailOrd.Quantity = item.Quantity;
                    detailOrd.unitPrice = item.unitPrice;
                    _db.OrderDetails.Add(detailOrd);
                    Product product = _db.Products.SingleOrDefault(p => p.Id == item.IdProduct);
                    product.Sold++;
                    Stock stock = _db.Stocks.SingleOrDefault(s => s.ProductId == item.IdProduct && s.ColorId == item.IdColor && s.SizeId == item.IdSize);
                    stock.Stock1--;
                    _db.Entry(stock).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                _db.SaveChanges();
                Session["Cart"] = null;
                sendPass(order);
                return RedirectToAction("confirmOrder", "Cart");
            }
            return View(model);
        }
        public ActionResult confirmOrder()
        {
            return View();
        }
        public void sendPass(Order order)

        {
            var orderDetail = _db.OrderDetails.Where(p => p.OrderId == order.Id).ToList();
            StringBuilder htmlBuilder = new StringBuilder();
            foreach (var item in orderDetail)
            {
                htmlBuilder.AppendLine(item.Stock.Product.Name + "-" + item.Stock.Color.Name + "-" + item.Stock.Size.Name + " x" + item.Quantity + "</p>\n" + "</br>");
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
                <h1>Đơn hàng mới !!! " + ",</h1>" +
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
    }
}