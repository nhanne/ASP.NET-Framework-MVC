using Boutique.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Order
        public ActionResult Index(string search, string sort, int page = 1)
        {
            var orderDetail = _db.OrderDetails.ToList();
            ViewBag.orderDetail = orderDetail.ToList();
            var query = _db.Orders.AsQueryable();
            if (!String.IsNullOrEmpty(search))
            {
                ViewBag.search = search;
                query = query.Where(p => p.Customer.Phone.ToString().ToLower().Contains(search.ToLower()));
            }
            query = Sort(sort, query);
            // Phân trang
            var orders = query.ToList();
            ViewBag.orderList = orders.ToPagedList(page, 10);
            var totalPages = (int)Math.Ceiling((double)query.Count() / 8);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View();
        }

        private IQueryable<Order> Sort(string sort, IQueryable<Order> query)
        {
            switch (sort)
            {
                case "Wait":
                    query = query.Where(s => s.Status == "Chưa giao hàng");
                    break;
                case "Deli":
                    query = query.Where(s => s.Status == "Đang giao hàng");
                    break;
                case "Done":
                    query = query.Where(s => s.Status == "Hoàn thành");
                    break;
                case "Cancel":
                    query = query.Where(s => s.Status == "Đã hủy");
                    break;
                default:
                    query = query;
                    break;
            }
            return query;
        }

        public ActionResult Detail(int? Id)
        {
            var orderDetail = _db.OrderDetails.ToList();
            ViewBag.orderDetail = orderDetail.ToList();
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = _db.Orders.Find(Id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
        public ActionResult handleOrder(int? Id)
        {
            var orderDetail = _db.OrderDetails.ToList();
            ViewBag.orderDetail = orderDetail.ToList();
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = _db.Orders.Find(Id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult handleOrder([Bind(Include = "Id")] Order model)
        {
            Order order = _db.Orders.Find(model.Id);

            if (order == null)
            {
                return HttpNotFound();
            }

            order.Status = "Đang giao hàng";
            order.DeliTime = DateTime.Now.AddDays(3);

            var orderDetails = _db.OrderDetails.Where(item => item.OrderId == order.Id).ToList();

            foreach (var item in orderDetails)
            {
                Stock product = _db.Stocks.SingleOrDefault(p => p.ProductId == item.Stock.ProductId && p.ColorId == item.Stock.ColorId && p.SizeId == item.Stock.SizeId);
                product.Stock1--;
                _db.Entry(product).State = EntityState.Modified;
                _db.SaveChanges();
            }
            _db.Entry(order).State = EntityState.Modified;
            _db.SaveChanges();
            sendPass(order);


            return RedirectToAction("Index");
        }
        public void sendPass(Order order)

        {
            var orderDetail = _db.OrderDetails.Where(p => p.OrderId == order.Id).ToList();
            StringBuilder htmlBuilder = new StringBuilder();
            foreach (var item in orderDetail)
            {
                htmlBuilder.AppendLine(item.Stock.Product.Name+"-"+item.Stock.Color.Name+"-"+item.Stock.Size.Name+" x"+item.Quantity+"</p>\n" + "</br>");
            }

            MailAddress fromAddress = new MailAddress("nhancmvn12@gmail.com", "Nhân Boutique");

            MailAddress toAddress = new MailAddress(order.Customer.Email.ToString());

            const string fromPassword = "jznrdhzrgpnsxswp";

            string subject = "Xác nhận đơn hàng #" + order.Id.ToString();
            string body = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Xác nhận đơn hàng</title>
            </head>
            <body>
                <h2>Xin chào " + order.Customer.FullName + ",</h2>" +
                "<p>Cảm ơn bạn đã đặt hàng tại Nhân Boutique. Dưới đây là thông tin về đơn hàng của bạn:</p>" +
                "<h3>Thông tin khách hàng:</h3>" +
                "<p><strong>Tên khách hàng:</strong> " + order.Customer.FullName + "</p>" +
                "<p><strong>Email:</strong> " + order.Customer.Email + "</p>" +
                "<p><strong>Địa chỉ:</strong> " + order.Address + "</p>" +
                "<p><strong>Số điện thoại:</strong> " + order.Customer.Phone + "</p>" +
                "<h3>Thông tin đơn hàng:</h3>" +
                htmlBuilder.ToString() +
                "<p><strong>Mã đơn hàng:</strong> " + order.Id + "</p>" +
                "<p><strong>Ngày đặt hàng:</strong> " + order.OrdTime?.ToString("dd-MM-yyyy") + "</p>" +
                "<p><strong>Tình trạng đơn hàng:</strong> " + order.Status + "</p>" +
                "<p><strong>Dự kiến giao hàng:</strong> " + order.DeliTime?.ToString("dd-MM-yyyy") + "</p>" +
                "<p><strong>Giá trị đơn hàng:</strong> " + string.Format("{0:#,0}", order.TotalPrice) + "VNĐ"+"</p>" +
                "<!-- Thêm thông tin khác về đơn hàng nếu cần -->" +
                "<p>Xin hãy kiểm tra lại thông tin đơn hàng của bạn. Nếu bạn có bất kỳ câu hỏi hoặc thắc mắc nào, hãy liên hệ với chúng tôi qua email nhancmvn12@gmail.com hoặc số điện thoại 0858032268.</p>" +
                "<p>Chúng tôi rất hân hạnh được phục vụ bạn!</p>" +
                "<p>Trân trọng,</p>" +
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