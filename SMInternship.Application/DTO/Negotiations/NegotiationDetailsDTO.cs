using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NegotiationDetailsDTO
    {
        public int ID { get; set; }
        [Required]
        [Range(0.01, Double.MaxValue)]
        public double Price { get; set; }
        public int AttempCounter { get; set; }
        public DateTime LastAttemp { get; set; }
        public NegotiationStatus Status { get; set; }
        public string NegotiationToken { get; set; }

        public string ProductName { get; set; }
        public int ProductID { get; set; }

    }
}
