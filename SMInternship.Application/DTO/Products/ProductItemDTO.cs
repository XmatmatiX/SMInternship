using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Products
{
    public class ProductItemDTO
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
