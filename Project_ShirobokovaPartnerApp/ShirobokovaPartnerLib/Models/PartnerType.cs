using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShirobokovaPartnerLib.Models
{
    [Table("shirobokova_partner_types", Schema = "app")]
    public class PartnerType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}