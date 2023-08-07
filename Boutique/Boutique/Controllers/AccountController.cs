using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Controllers
{
    public class AccountController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Account
        public ActionResult Index()
        {
            if(Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == ""){
                return RedirectToAction("Login", "Account");
            }
            Customer khsession = (Customer)Session["Taikhoan"];
            Customer customer = _db.Customers.Find(khsession.Id);
            return View(customer);
        }
        // Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Customers.Where(s => s.Email.Equals(email) && s.Password.Equals(password)).ToList();
                Customer kh = _db.Customers.SingleOrDefault(s => s.Email.Equals(email) && s.Password.Equals(password) && s.Member == true);
                if (kh != null)
                {
                    Session["Taikhoan"] = kh;
                }
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullNamekh"] = data.FirstOrDefault().FullName;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        // Registery
        public ActionResult Register()
        {
            return View();
        }
        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer khachhang)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Customers.FirstOrDefault(s => s.Email == khachhang.Email && khachhang.Member == true);
                if (check == null)
                {
                    khachhang.Member = true;
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Customers.Add(khachhang);
                    _db.SaveChanges();
                }

                else
                {
                    Customer customer = _db.Customers.FirstOrDefault(s => s.Email.Equals(khachhang.Email));
                    customer.Member = true;
                    customer.Phone = khachhang.Phone;
                    customer.Password = khachhang.Password;
                    customer.FullName = khachhang.FullName;
                    _db.Entry(customer).State = EntityState.Modified;
                    _db.SaveChanges();
                    //ViewBag.error = "Email đã được đăng ký ở tài khoản khác";
                }
                return RedirectToAction("Login");
            }
            return View(khachhang);
        }

        public ActionResult edit(int Id)
        {
            Customer khsession = (Customer)Session["Taikhoan"];
            Customer kh = _db.Customers.Find(Id);
            if (khsession.Id != kh.Id || khsession == null)
            {
                return RedirectToAction("Index");

            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(Customer model, HttpPostedFileBase file)
        {
            Customer customer = _db.Customers.Find(model.Id);
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string picture = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/customer"), picture);
                    file.SaveAs(path);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                    customer.Avatar = picture;
                }
                customer.FullName = model.FullName;
                customer.Email = model.Email;
                customer.Phone = model.Phone;
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult orderList()
        {
            Customer khsession = (Customer)Session["Taikhoan"];
            if(khsession == null)
            {
                return RedirectToAction("Login");
            }
            Customer kh = _db.Customers.Find(khsession.Id);
            var orders = _db.Orders.Where(o => o.CustomerId == kh.Id).ToList();
            ViewBag.orders = orders;
            return View();
        }
        public ActionResult getOrder(int Id)
        {
            Order item = _db.Orders.Find(Id);
            if (item != null)
            {
                var customer = _db.Customers.Find(item.CustomerId);
                if(customer != null)
                {
                    string formattedOrdTime = item.OrdTime?.ToString("dd-MM-yyyy");
                    var data = new
                    {
                        Customer = new
                        {
                            customer.Id,
                            customer.FullName,
                            customer.Email,
                            customer.Phone
                        },
                        item.Id,
                        item.Address,
                        item.Status,
                        TotalPrice = string.Format("{0:#,0}", item.TotalPrice),
                        OrdTime = formattedOrdTime
                    };

                    var detail = _db.OrderDetails.Where(i => i.OrderId == item.Id).ToList();
                    var detailList = detail.Select(s => new 
                    {  
                        s.Quantity,
                        unitPrice = string.Format("{0:#,0}", s.unitPrice),
                        Stock = new
                        {
                            Product = s.Stock.Product.Name,
                            Color = s.Stock.Color.Name,
                            Size = s.Stock.Size.Name
                        }
                    });
                    return Json(new { data = data, detailList = detailList, totalDetail = detailList.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        
    }
}