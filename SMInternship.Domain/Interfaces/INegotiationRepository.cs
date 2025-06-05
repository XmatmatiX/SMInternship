using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Interfaces
{
    public interface INegotiationRepository
    {
        int AddNegotiation(Negotiation negotiation);
        int UpdateNegotiation(Negotiation negotiation);
        Negotiation GetNegotiation(int id);
        Negotiation GetNegotiationByToken(string token);
        IQueryable<Negotiation> GetNegotiations();
        IQueryable<Negotiation> GetNegotiationsWithStatus(NegotiationStatus status);
        bool isTokenTaken(string Token);

    }
}
