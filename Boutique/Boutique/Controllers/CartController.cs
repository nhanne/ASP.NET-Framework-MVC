using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boutique.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> listCart = getCart();
            ViewBag.totalPrice = TongTien();
            ViewBag.totalQuantity = totalQuantity();
            return View(listCart);
        }

        public List<Cart> getCart()
        {
            List<Cart> listCart = Session["Cart"] as List<Cart>;

            if (listCart == null)
            {
                listCart = new List<Cart>();
                Session["Cart"] = listCart;

            }
            return listCart;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult addtoCart(int IdProduct, string strURL)
        {
            // Lấy ra session giỏ hàng
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            Cart product = listCart.Find(n => n.IdProduct == IdProduct);
            if (product == null)
            {
                product = new Cart(IdProduct);
                listCart.Add(product);
            }
            else
            {
                product.Quantity++;
            }
            ViewBag.noti = "Đã thêm sản phẩm vào giỏ hàng";
            return Content(strURL);
        }
        // Tính tổng số lượng sản phẩm
        public int totalQuantity()
        {
            int itotalQuantity = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                itotalQuantity = listCart.Sum(n => n.Quantity);
            }
            return itotalQuantity;
        }
        //Tính tổng tiền sản phẩm
        private double TongTien()
        {
            double iTongTien = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                iTongTien = listCart.Sum(n => n.totalPrice);
            }
            return iTongTien;
        }
        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult deletefromCart(int IdProduct)
        {
            // Lấy giỏ hàng từ session
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            Cart product = listCart.SingleOrDefault(n => n.IdProduct == IdProduct);
            //nếu tồn tại thì sửa số lượng
            if (product != null)
            {
                listCart.RemoveAll(n => n.IdProduct == IdProduct);
                return RedirectToAction("Index");
            }
            if (listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }
        // Cập nhật giỏ hàng
        public ActionResult UpdateCart(int IdProduct, FormCollection f)
        {
            // Lấy giỏ hàng từ session
            List<Cart> listCart = getCart();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            Cart product = listCart.SingleOrDefault(n => n.IdProduct == IdProduct);
            if (product != null)
            {
                product.Quantity = int.Parse(f["quantity"].ToString());
            }
            return RedirectToAction("Index");
        }
    }
}