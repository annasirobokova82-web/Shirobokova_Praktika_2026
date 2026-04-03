using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShirobokovaPartnerLib.Models
{
    [Table("shirobokova_sales", Schema = "app")]
    public class Sale
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}