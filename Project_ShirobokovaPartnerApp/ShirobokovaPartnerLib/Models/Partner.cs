using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShirobokovaPartnerLib.Models
{
    [Table("shirobokova_partners", Schema = "app")]
    public class Partner
    {
        public int Id { get; set; }
        public int PartnerTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Address { get; set; }
        public string? DirectorName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual PartnerType? PartnerType { get; set; }

        [NotMapped]
        public decimal TotalSales { get; set; }

        [NotMapped]
        public int DiscountPercent
        {
            get
            {
                if (TotalSales > 300000) return 15;
                if (TotalSales > 50000) return 10;
                if (TotalSales > 10000) return 5;
                return 0;
            }
        }
    }
}