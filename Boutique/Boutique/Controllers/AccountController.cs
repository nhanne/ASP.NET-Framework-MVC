using Boutique.Models;
using System;
using System.Collections.Generic;
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
            return View();
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
                Customer kh = _db.Customers.SingleOrDefault(s => s.Email.Equals(email) && s.Password.Equals(password));
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
                var check = _db.Customers.FirstOrDefault(s => s.Email == khachhang.Email || s.Phone == khachhang.Phone);
                if (check == null)
                {
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Customers.Add(khachhang);
                    _db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email đã được đăng ký ở tài khoản khác";
                    return View();
                }
            }
            return View();
        }
    }
}