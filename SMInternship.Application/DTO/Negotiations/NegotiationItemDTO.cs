using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NegotiationItemDTO
    {
        public int ID { get; set; }
        public DateTime LastAttemp { get; set; }
        public string Status { get; set; }
        

        public string ProductName { get; set; }
        public int ProductID { get; set; }
    }
}
