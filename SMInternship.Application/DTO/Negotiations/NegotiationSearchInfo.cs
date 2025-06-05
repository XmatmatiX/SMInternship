using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NegotiationSearchInfo
    {
        public string? SearchingStatus { get; set; }
        public string? SearchingProduct { get; set; }


        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
