using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net;

namespace Boutique.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Category
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var dsCate = _db.Categories.ToList().OrderBy(n => n.Id).ToPagedList(pageNumber, pageSize);
            ViewBag.dsCate = dsCate;
            return View(dsCate);
        } 


        public ActionResult Create()
        {
            return View(new Category() { Id = 0, Code = "", Name = "" });
        }
        [HttpPost]
        public ActionResult Create(Category model)
        {
            var catalogs = (from s in _db.Categories select s).ToList();
            model.Id = catalogs.Last().Id + 1;
            // lưu dữ liệu vào db
            if (string.IsNullOrEmpty(model.Code) && string.IsNullOrEmpty(model.Name) == true)
            {
                ModelState.AddModelError("", "Mã và tên danh mục không được để trống");
                return View(model);
            }
            _db.Categories.Add(model);
            _db.SaveChanges();
            if (model.Id > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Không lưu vào được database");
                return View(model);
            }
        }

        public ActionResult Delete(int Id)
        {

            if (Id.ToString() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category cate = _db.Categories.Find(Id);
            if (cate == null)
            {
                return HttpNotFound();
            }
            return View(cate);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            try
            {
                Category cate = _db.Categories.Find(Id);
                _db.Categories.Remove(cate);
                _db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }
    }
}