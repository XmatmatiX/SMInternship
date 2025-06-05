using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NewNegotiationDTO
    {
        [Range(0.01, Double.MaxValue)]
        public double Price { get; set; }
        public int ProductID { get; set; }
    }
}
