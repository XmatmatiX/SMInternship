using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Negotiations
{
    public class NegotiationListDTO
    {
        public List<NegotiationItemDTO> NegotiationList { get; set; }

        public int Count { get; set; }
        public NegotiationStatus? SearchingStatus { get; set; }
        public string? SearchingProduct { get; set; }
        public int ActualPage { get; set; }
        public int Pages { get; set; }
        public int PageSize { get; set; }


        public NegotiationListDTO()
        {
            NegotiationList = new List<NegotiationItemDTO>();
        }
    }
}
