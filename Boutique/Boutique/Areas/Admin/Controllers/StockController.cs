using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
                return RedirectToAction("Color", "Stock");
            }
            return View(model);
        }
        public ActionResult deleteColor(int Id)
        {

            if (Id.ToString() == null)
            {
                return RedirectToAction("Color", "Stock");
            }
            Color color = _db.Colors.Find(Id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }
        [HttpPost, ActionName("deleteColor")]
        [ValidateAntiForgeryToken]
        public ActionResult confirmXcolor(int Id)
        {
                Color color = _db.Colors.Find(Id);
                _db.Colors.Remove(color);
                _db.SaveChanges();
            return RedirectToAction("Color", "Stock");
        }
        public ActionResult editColor(int Id)
        {
            if (Id.ToString() == null)
            {
                return RedirectToAction("Color", "Stock");
            }
            Color color = _db.Colors.Find(Id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editColor(Color model)
        {
            Color color = _db.Colors.Find(model.Id);
            if (ModelState.IsValid)
            {
                color.Name = model.Name;
                color.Ghichu = model.Ghichu;
                _db.Entry(color).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Color", "Stock");
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