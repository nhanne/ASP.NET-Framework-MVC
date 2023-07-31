using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Areas.Admin.Controllers
{
    public class PromotionController : Controller
    {
        BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Promotion
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetData()
        {
            var result = _db.Promotions.ToList();
            return Json(new { Data = result, TotalItems = result.Count }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetById(int Id)
        {
            var item = _db.Promotions.Find(Id);
            return Json(new { data = item }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Create(Promotion model)
        {
            if (model.promotion_id != 0) {
                var promotion = _db.Promotions.Find(model.promotion_id);
                promotion.promotion_name = model.promotion_name;
                promotion.description = model.description;
                promotion.start_date = model.start_date;
                promotion.end_date = model.end_date;
                promotion.discount_percentage = model.discount_percentage;
             
                _db.Entry(promotion).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                _db.Promotions.Add(model);
                try
                {
                    _db.SaveChanges();
                    return Json(new { success = true });

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors = errors });
                }
            }
        }
        [HttpPost]
        public ActionResult Delete(int promotion_id)
        {
            var promotion = _db.Promotions.Find(promotion_id);
            _db.Promotions.Remove(promotion);
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}