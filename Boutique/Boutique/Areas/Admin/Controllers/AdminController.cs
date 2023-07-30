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