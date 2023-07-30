using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Boutique.Models;
using PagedList;

namespace Boutique.Controllers
{
    public class HomeController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        public ActionResult Index()
        {
            var dsProduct = from p in _db.Products select p;
            ViewBag.dsProduct = dsProduct.OrderByDescending(p => p.Sold).Take(4).ToList();
            return View();
        }
        public ActionResult Store(string sort, string category, string search, int pageIndex = 1)
        {
            var dsCate = _db.Categories.ToList();
            ViewBag.dsCate = dsCate.ToList();
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());
                ViewBag.Cate = category;
            }
            if (!String.IsNullOrEmpty(search))
            {
                ViewBag.Search = search;
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            query = Sort(sort, query);
            // Phân trang
            var products = query.ToList();
            ViewBag.dsProduct = products.ToPagedList(pageIndex, 16);
            var totalPages = (int)Math.Ceiling((double)query.Count() / 16);
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalPages = totalPages;
            return View();
        }
        private IQueryable<Product> Sort(string sort, IQueryable<Product> query)
        {
            switch (sort)
            {
                case "asc":
                    query = query.OrderBy(c => c.unitPrice);
                    ViewBag.currentSort = "Giá: thấp đến cao";
                    break;
                case "desc":
                    query = query.OrderByDescending(c => c.unitPrice);
                    ViewBag.currentSort = "Giá: cao đến thấp";
                    break;
                case "new":
                    query = query.OrderByDescending(c => c.stockInDate);
                    ViewBag.currentSort = "Sản phẩm mới nhất";
                    break;
                case "hot":
                    query = query.OrderByDescending(c => c.Sold);
                    ViewBag.currentSort = "Sản phẩm bán chạy";
                    break;
                case "sale":
                    query = query.OrderByDescending(c => c.Sale);
                    ViewBag.currentSort = "Sản phẩm khuyến mãi";
                    break;
                default:
                    query = query.OrderByDescending(c => c.Sold);
                    ViewBag.currentSort = "";
                    break;
            }

            ViewBag.Sort = sort;

            return query;
        }
      
        public ActionResult Product(int Id)
        {
            Product product = _db.Products.SingleOrDefault(p => p.Id == Id);
            var stocks = _db.Stocks.Where(p => p.ProductId == Id).Include(s=>s.Size).Include(s=>s.Color);
            var size = stocks.Select(s=> s.Size).Distinct().ToList();
            var color = stocks.Select(s => s.Color).Distinct().ToList();
            ViewBag.size = size;
            ViewBag.color = color;
            ViewBag.dsCate = _db.Categories.ToList();
            return View(product);
        }
        //get product in stock
        [HttpGet]
        public ActionResult getProductbyId(int ProductId, int SizeId, int ColorId)
        {
            Stock item = _db.Stocks.FirstOrDefault(p => p.ProductId == ProductId && p.SizeId == SizeId && p.ColorId == ColorId);
            if(item != null)
            {
                return Json(new { Data = item.Stock1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Data = "Sản phẩm hiện đang hết hàng" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Contact(string name, string email, string messenger)
        {
            MailAddress fromAddress = new MailAddress(email, name);

            MailAddress toAddress = new MailAddress("nhancmvn12@gmail.com");
            const string fromPassword = "jznrdhzrgpnsxswp";

            string subject = "Hỗ trợ vấn đề";
            string body = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Xác </title>
            </head>
            <body>
                <p><strong>Người liên hệ: </strong>" + name + "</p>" +
                "<p><strong>Email: </strong> " + email + "</p>" +
                "<p><strong>Nội dung: </strong> " + messenger + "</p>" +
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
            return Json(new { content = "Chúng tôi sẽ sớm liên lạc lại với bạn qua email: " + email });
        }

    }
}