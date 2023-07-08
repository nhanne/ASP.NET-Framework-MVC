using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boutique.Models;
using PagedList;

namespace Boutique.Controllers
{
    public class HomeController : Controller
    {
        private BoutiqueEntities _db = new BoutiqueEntities();
        public ActionResult Index()
        {
            var dsProduct = from p in _db.Products select p;
            ViewBag.dsProduct = dsProduct.OrderByDescending(p=>p.stockInDate).Take(8).ToList();
            return View();
        }
        public ActionResult Store(string sort,string category, string search, int pageIndex = 1)
        {
            var dsCate = _db.Categories.ToList();
            ViewBag.dsCate = dsCate.ToList();
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());
                ViewBag.Cate = category;
            }
            if (!String.IsNullOrEmpty(search))
            {
                ViewBag.Search = search;
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }
            
            query = Sort(sort, query);
            var products = query.ToList();
            ViewBag.dsProduct = products.ToPagedList(pageIndex, 8);
            // Phân trang
            var totalPages = (int)Math.Ceiling((double)query.Count() / 8);
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalPages = totalPages;
            return View();
        }
        private IQueryable<Product> Sort(string sort, IQueryable<Product> query)
        {
            switch (sort)
            {
                case "asc":
                    query = query.OrderBy(c => c.unitPrice);
                    ViewBag.currentSort = "Giá: thấp đến cao";
                    break;
                case "desc":
                    query = query.OrderByDescending(c => c.unitPrice);
                    ViewBag.currentSort = "Giá: cao đến thấp";
                    break;
                case "new":
                    query = query.OrderByDescending(c => c.stockInDate);
                    ViewBag.currentSort = "Sản phẩm mới nhất";
                    break;
                case "hot":
                    query = query.OrderByDescending(c => c.Sold);
                    ViewBag.currentSort = "Sản phẩm bán chạy";
                    break;
                case "sale":
                    query = query.OrderByDescending(c => c.Sale);
                    ViewBag.currentSort = "Sản phẩm khuyến mãi";
                    break;
                default:
                    query = query.OrderByDescending(c => c.Sold);
                    ViewBag.currentSort = "";
                    break;
            }

            ViewBag.Sort = sort;

            return query;
        }
    }
}