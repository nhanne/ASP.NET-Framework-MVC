using Boutique.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Boutique.Controllers
{
    public class CartController : Controller
    {
        BoutiqueEntities _db = new BoutiqueEntities();
        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> listCart = getCart();
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
            ViewBag.totalPrice = TongTien();
            ViewBag.totalQuantity = totalQuantity();
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
        public ActionResult CheckOut()
        {
            List<Cart> listCart = getCart();
            if (Session["Cart"] == null || listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Cart = listCart;
            Customer model = (Customer)Session["Taikhoan"];
            if(model == null)
            {
                model = new Customer();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckOut(Customer model,FormCollection f)
        {
            getCart();
            var products = _db.Products.ToList();
            var address = f["address"];
            var note = f["note"];
            var check = _db.Customers.FirstOrDefault(s => s.Email.Equals(model.Email));
            ModelState.Remove("Password");
            if (ModelState.IsValid) 
            {
                if (check == null)
                {
                    Customer customer = new Customer();
                    customer.Password = "Nhan123?";
                    customer.FullName = model.FullName;
                    customer.Phone = model.Phone;
                    customer.Email = model.Email;
                    customer.Member = false;
                    _db.Customers.Add(customer);
                    _db.SaveChanges();
                }
                Customer cus = _db.Customers.SingleOrDefault(s => s.Email.Equals(model.Email));
                Order order = new Order();
                order.CustomerId = cus.Id;
                order.OrdTime = DateTime.Now;
                order.DeliTime = order.OrdTime.Value.AddDays(3);
                order.Status = "Chưa giao hàng";
                order.Payment = false;
                order.Address = address.ToString();
                order.TotalPrice = TongTien();
                order.TotalQuantity = totalQuantity();
                order.Note = note.ToString();
                _db.Orders.Add(order);
                _db.SaveChanges();
                List<Cart> listCart = getCart();
                foreach (var item in listCart)
                {
                    OrderDetail detailOrd = new OrderDetail();
                    detailOrd.OrderId = order.Id;
                    detailOrd.ProductId = item.IdProduct;
                    detailOrd.Quantity = item.Quantity;
                    detailOrd.unitPrice = item.unitPrice;
                    _db.OrderDetails.Add(detailOrd);
                }
                _db.SaveChanges();
                Session["Cart"] = null;
                return RedirectToAction("confirmOrder", "Cart");
            }
            return View(model);
        }
        public ActionResult confirmOrder()
        {
            return View();
        }
    }
}