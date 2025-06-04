using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Infrastructure.Repositories
{
    public class NegotiationRepository : INegotiationRepository
    {
        private readonly Context _context;

        public NegotiationRepository(Context context)
        {
            _context = context;
        }

        public int CreateNegotiation(Negotiation negotiation)
        {
            var neg = _context.Negotiations.Add(negotiation);
            _context.SaveChanges();
            return neg.Entity.ID;
        }

        public Negotiation GetNegotiation(int id)
        {
            Negotiation negotiation = _context.Negotiations.Where(n => n.ID == id).FirstOrDefault();

            return negotiation;
        }

        public Negotiation GetNegotiationByToken(string token)
        {
            Negotiation negotiation = _context.Negotiations.Where(n => n.NegotiationToken == token).FirstOrDefault();

            return negotiation;
        }

        public int UpdateNegotiation(Negotiation negotiation)
        {
            _context.Attach(negotiation);
            _context.Entry(negotiation).Property(n => n.IsActive).IsModified = true;
            _context.Entry(negotiation).Property(n => n.Status).IsModified = true;
            _context.Entry(negotiation).Property(n => n.AttempCounter).IsModified = true;
            _context.Entry(negotiation).Property(n => n.LastAttemp).IsModified = true;
            _context.Entry(negotiation).Property(n => n.Price).IsModified = true;

            _context.SaveChanges();
            return negotiation.ID;

        }
    }
}
