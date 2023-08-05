using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Admin
        public ActionResult Index()
        {
            return View();
        }

        //Login 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Staff objStaff)
        {
            if (ModelState.IsValid)
            {
                var obj = _db.Staffs.Where(a => a.Email.Equals(objStaff.Email) && a.Password.Equals(objStaff.Password)).FirstOrDefault();
                if (obj != null && obj.Job_title == 1)
                {
                    Session["NV"] = obj;
                    return RedirectToAction("Index", "Admin");
                }
                else if (obj != null && obj.Job_title == 2)
                {
                    Session["NV"] = obj;
                    return RedirectToAction("Index", "Order");
                }
                else
                {
                    ModelState.AddModelError("", "Không phải là tài khoản quản trị viên");
                }
            }
            return View(objStaff);  
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["NV"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        public ActionResult Logout()
        {
            Session["NV"] = null;
            return RedirectToAction("Login", "Admin");
        }

        // Thống kê doanh thu
        public ActionResult getOrders()
        {
            bool proxyCreation = _db.Configuration.ProxyCreationEnabled;
            try
            {
                _db.Configuration.ProxyCreationEnabled = false;

                //your stuff
                var result = _db.Orders.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            finally
            {
                //restore ProxyCreation to its original state
                _db.Configuration.ProxyCreationEnabled = proxyCreation;
            }
        }
    }
}