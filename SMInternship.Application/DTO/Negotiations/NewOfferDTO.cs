using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NewOfferDTO
    {
        public string Token { get; set; }
        [Required]
        [Range(0.01, Double.MaxValue)]
        public double Price { get; set; }
    }
}
