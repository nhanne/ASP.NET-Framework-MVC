//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Boutique.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Promotion
    {
        public int promotion_id { get; set; }
        public string promotion_name { get; set; }
        public string description { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public decimal discount_percentage { get; set; }
    }
}
