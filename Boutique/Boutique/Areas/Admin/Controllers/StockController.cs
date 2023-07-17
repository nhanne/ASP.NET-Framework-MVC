using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Areas.Admin.Controllers
{
    public class StockController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Stock
        public ActionResult Index()
        {
            var stocks = _db.Stocks.OrderBy(p=>p.ProductId).ToList();
            ViewBag.stocks = stocks;
            return View();
        }
        //Color
        public ActionResult Color()
        {
            var colors = _db.Colors.ToList();
            ViewBag.colors = colors;
            return View();
        }
        public ActionResult newColor()
        {
            return View(new Color() { Id = 0, Name = "", Ghichu = "" });
        }
        [HttpPost]
        public ActionResult newColor(Color model)
        {
          
            // lưu dữ liệu vào db
            if (ModelState.IsValid)
            {
                _db.Colors.Add(model);
                _db.SaveChanges();
            }
            return View(model);
        }
        //Size
        public ActionResult Size()
        {
            var sizes = _db.Sizes.ToList();
            ViewBag.sizes = sizes;
            return View();
        }
    }
}