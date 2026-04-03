using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShirobokovaPartnerLib.Models
{
    [Table("shirobokova_products", Schema = "app")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}