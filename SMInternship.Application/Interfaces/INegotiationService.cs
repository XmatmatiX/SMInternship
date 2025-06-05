using SMInternship.Application.DTO.Negotiations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Interfaces
{
    public interface INegotiationService
    {
        string AddNegotiation(NewNegotiationDTO dto);
        int SendNewOffer(NewOfferDTO dto);
        NegotiationListDTO GetNegotiations(NegotiationSearchInfo info);
        NegotiationDetailsDTO GetNegotiationDetails(int id);
        NegotiationDetailsDTO GetNegotiationDetails(string token);
        int ResponseToOffer(ResponseDTO dto);
        
    }
}
