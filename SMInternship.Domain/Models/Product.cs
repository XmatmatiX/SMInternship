using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Negotiation> Negotiations { get; set; }
    }
}
