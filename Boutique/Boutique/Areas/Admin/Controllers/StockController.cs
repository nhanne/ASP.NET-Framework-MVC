using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Boutique.Areas.Admin.Controllers
{
    public class StockController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Stock
        public ActionResult Index(string search)
        {
            var stocks = _db.Stocks.AsQueryable();
            if (!String.IsNullOrEmpty(search))
            {
                stocks = stocks.Where(p => p.Product.Code.Contains(search));
            }
            else
            {
                stocks = stocks.OrderBy(p => p.ProductId);
            }
            ViewBag.stocks = stocks.ToList();
            return View();
        }

        public ActionResult GetStockData()
        {
            var result = _db.Stocks.ToList();
            var jsonResult = JsonConvert.SerializeObject(result);
            return Content(jsonResult, "application/json");
        }
        //Color

        public ActionResult Color()
        {
            return View();
        }
        public ActionResult loadColor()
        {
            var colorList = _db.Colors.ToList();
            var validColorList = colorList.Select(c => new
            {
                Id = c.Id,
                Name = !string.IsNullOrEmpty(c.Name) ? c.Name : "Tên màu không hợp lệ",
                Ghichu = !string.IsNullOrEmpty(c.Ghichu) ? c.Ghichu : "Chú thích không hợp lệ"
            }).ToList();
            return Json(new
            {
                Data = validColorList,
                TotalItems = validColorList.Count,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getColor(int Id)
        {
            var color = _db.Colors.Find(Id);
            var validColor = new
            {
                Id = color.Id,
                Name = !string.IsNullOrEmpty(color.Name) ? color.Name : "Tên màu không hợp lệ",
                Ghichu = !string.IsNullOrEmpty(color.Ghichu) ? color.Ghichu : "Chú thích không hợp lệ"
            };

            return Json(new
            {
                data = validColor
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult createColor(Color model)
        {
            if (model.Id != 0)
            {
                var color = _db.Colors.Find(model.Id);
                color.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : color.Name;
                color.Ghichu = !string.IsNullOrEmpty(model.Ghichu) ? model.Ghichu : color.Name;
                _db.Entry(color).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                _db.Colors.Add(model);
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
        public ActionResult deleteColor(int Id)
        {
            var color = _db.Colors.Find(Id);
            _db.Colors.Remove(color);
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }





        //Size
        public ActionResult Size()
        {
            return View();
        }
        public ActionResult loadSize()
        {
            var sizeList = _db.Sizes.ToList();
            var validSizeList = sizeList.Select(c => new
            {
                Id = c.Id,
                Name = !string.IsNullOrEmpty(c.Name) ? c.Name : "Tên kích thước không hợp lệ",
                Ghichu = !string.IsNullOrEmpty(c.Ghichu) ? c.Ghichu : "Chú thích không hợp lệ"
            }).ToList();
            return Json(new
            {
                Data = validSizeList,
                TotalItems = validSizeList.Count,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSize(int Id)
        {
            var size = _db.Sizes.Find(Id);
            var validSize = new
            {
                Id = size.Id,
                Name = !string.IsNullOrEmpty(size.Name) ? size.Name : "Tên màu không hợp lệ",
                Ghichu = !string.IsNullOrEmpty(size.Ghichu) ? size.Ghichu : "Chú thích không hợp lệ"
            };

            return Json(new
            {
                data = validSize
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult deleteSize(int Id)
        {
            var size = _db.Sizes.Find(Id);
            _db.Sizes.Remove(size);
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        [HttpPost]
        public ActionResult createSize(Size model)
        {
            if (model.Id != 0)
            {
                var size = _db.Sizes.Find(model.Id);
                size.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : size.Name;
                size.Ghichu = !string.IsNullOrEmpty(model.Ghichu) ? model.Ghichu : size.Name;
                _db.Entry(size).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                _db.Sizes.Add(model);
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
    }
}