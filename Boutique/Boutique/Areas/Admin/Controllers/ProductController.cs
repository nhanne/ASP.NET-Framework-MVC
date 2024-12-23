﻿using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Net;
using System.Data.Entity;

namespace Boutique.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {   
        private BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Admin/Product
        public ActionResult Index(string searchString, int pageIndex = 1)
        {
            var query = _db.Products.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                ViewBag.search = searchString;
                query = query.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }
            var products = query.ToList();
            var totalPages = (int)Math.Ceiling((double)query.Count() / 10);
            ViewBag.dsProduct = products.ToPagedList(pageIndex, 10);
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalPages = totalPages;
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_db.Categories, "ID", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, [Bind(Include = "CategoryId,Name," +
            "costPrice,Sale")] Product product)
        {
            if (ModelState.IsValid)
            {
                String anh = null;
                if(file != null)
                {
                    string picture = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                          Server.MapPath("~/Images/sp"), picture);
                    file.SaveAs(path);
                    anh = picture;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }
                product.Picture = anh;
                Random prCode = new Random();
                product.Code = prCode.Next(72000000,79000000).ToString();
                product.Sold = 0;
                product.unitPrice = (product.Sale != null) ? (product.unitPrice = (product.costPrice - (product.costPrice * product.Sale) / 100)) : (product.unitPrice = product.costPrice);                       
                _db.Products.Add(product);
                _db.SaveChanges();
               
                return RedirectToAction("AddPhanLoai", new {Id = product.Id });
            }
            ViewBag.CategoryId = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public ActionResult Edit(int Id)
        {
            if (Id.ToString() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _db.Products.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,Picture,Name,Code,costPrice,Quantity,Sold,Sale")] Product product, HttpPostedFileBase file)
        {
            Product pr = _db.Products.Find(product.Id);
            ViewBag.CatalogId = new SelectList(_db.Categories, "Id", "Name", pr.CategoryId);
            if (ModelState.IsValid)
            {
                String anh = pr.Picture;
                if (file != null)
                {
                    string picture = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                            Server.MapPath("~/Images/sp"), picture);
                    file.SaveAs(path);
                    anh = picture;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }
                pr.Picture = anh;
                pr.CategoryId = product.CategoryId;
                pr.Name = product.Name;
                pr.Code = product.Code;
                pr.costPrice = product.costPrice;
                pr.unitPrice = (product.Sale != null) ? (product.unitPrice = (product.costPrice - (product.costPrice * product.Sale) / 100)) : (product.unitPrice = product.costPrice);
                pr.Sold = product.Sold;
                pr.Sale = product.Sale;
                _db.Entry(pr).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        public ActionResult listIteminStock(int Id)
        {
            var stock = _db.Stocks.Where(p => p.ProductId == Id);
            var validitemStock = stock.Select(c => new
            {
                Id = c.Id,
                ProductId = (c.ProductId != 0) ? c.ProductId : -1,
                ColorId = (c.ColorId != 0) ? c.ColorId : -1,
                SizeId = (c.SizeId != 0) ? c.SizeId : -1,
                Stock1 = (c.Stock1 != 0) ? c.Stock1 : -1,
                stockInDate = (c.stockInDate != DateTime.MinValue) ? c.stockInDate : DateTime.MinValue
            }).ToList();
            return Json(new
            {
                Data = validitemStock,
                TotalItems = validitemStock.Count,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PhanLoaiSP(int Id)
        {
            var stock = _db.Stocks.Where(p => p.ProductId == Id);
            if (stock == null)
            {
                return RedirectToAction("AddPhanLoai", "Product", new { Id = Id });
            }
            ViewBag.id = Id;
            ViewBag.stock = stock.ToList();
            return View();
        }
        public ActionResult AddPhanLoai(int Id)
        {
            Stock model = _db.Stocks.FirstOrDefault(p => p.ProductId == Id);
            if (model == null)
            {
                model = new Stock{ProductId = Id};
            }
            ViewBag.id = Id;
            ViewBag.ColorId = new SelectList(_db.Colors, "Id", "Name");
            ViewBag.SizeId = new SelectList(_db.Sizes, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhanLoai(Stock model)
        {
            ViewBag.ColorId = new SelectList(_db.Colors, "Id", "Name", model.ColorId);
            ViewBag.SizeId = new SelectList(_db.Sizes, "Id", "Name", model.SizeId);
            if (ModelState.IsValid)
            {
                Stock stock = _db.Stocks.FirstOrDefault(p=> p.ProductId == model.ProductId && p.ColorId == model.ColorId && p.SizeId == model.SizeId);
                if(stock != null)
                {
                    ViewBag.error = "Phân loại hàng này đã tồn tại, vui lòng kiểm tra lại.";
                    return View(model);
                }
                else
                {
                    _db.Stocks.Add(model);
                    _db.SaveChanges();
                    return RedirectToAction("PhanLoaiSP", new { Id = model.ProductId });
                }
                
            }
            return View(model);
        }
        public ActionResult EditPhanLoai(int Id)
        {
            if (Id.ToString() == null)
            {
                return RedirectToAction("Index", "Product");
            }
            Stock stock = _db.Stocks.FirstOrDefault(p => p.Id == Id);
            if(stock == null)
            {
                return RedirectToAction("Index");
            }
            return View(stock);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhanLoai(Stock model)
        {
            Stock stock = _db.Stocks.Find(model.Id);
            if (ModelState.IsValid)
            {
                
                stock.Stock1 = model.Stock1;
                stock.stockInDate = DateTime.Now;
                _db.Entry(stock).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("PhanLoaiSP", new { Id = stock.ProductId });
            }
            return View(model);
        }
        public ActionResult DeletePhanLoai(int Id)
        {
            if (Id.ToString() == null)
            {
                return RedirectToAction("Index", "Product");
            }
            Stock stock = _db.Stocks.FirstOrDefault(p => p.Id == Id);
            if (stock == null)
            {
                return RedirectToAction("Index");
            }
            return View(stock);
        }
        [HttpPost, ActionName("DeletePhanLoai")]
        [ValidateAntiForgeryToken]
        public ActionResult confirmXPhanLoai(int Id)
        {
            Stock stock = _db.Stocks.FirstOrDefault(p => p.Id == Id);
            _db.Stocks.Remove(stock);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}