using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boutique.Models;

namespace Boutique.Controllers
{   
    public class HomeController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        public ActionResult Index()
        {
            var dsProduct = from p in _db.Products select p;
            ViewBag.dsProduct = dsProduct.ToList();
            return View();
        }
        public ActionResult Store()
        {
          
            return View();
        }
    }
}