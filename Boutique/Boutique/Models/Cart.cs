using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boutique.Models
{
    public class Cart
    {
        BoutiqueEntities _db = new BoutiqueEntities();
        public int IdProduct { set; get; }
        public string ProductName { set; get; }
        public string Picture { set; get; }
        public float unitPrice { set; get; }
        public int Quantity { set; get; }
        public string Brand { set; get; }
        public float totalPrice
        {
            get { return Quantity * unitPrice; }
        }
        public Cart(int MaSP)
        {
            IdProduct = MaSP;
            Product product = _db.Products.Single(n => n.Id == IdProduct);
            ProductName = product.Name;
            Picture = product.Picture;
            unitPrice = float.Parse(product.unitPrice.ToString());
            Quantity = 1;
            Brand = product.Category.Name;
        }
    }
}