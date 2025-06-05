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


        /// <summary>
        /// Add negotiation to database
        /// </summary>
        /// <param name="negotiation"></param>
        /// <returns>ID of created negotiation</returns>
        public int AddNegotiation(Negotiation negotiation)
        {
            var neg = _context.Negotiations.Add(negotiation);
            _context.SaveChanges();
            return neg.Entity.ID;
        }

        /// <summary>
        /// Return negotiation with given ID
        /// </summary>
        /// <param name="id">Negotiation ID</param>
        /// <returns></returns>
        public Negotiation GetNegotiation(int id)
        {
            Negotiation negotiation = _context.Negotiations.Where(n => n.ID == id).FirstOrDefault();

            if (negotiation == null)
                return null;

            // if negotiation is soft deleted return null
            if (!negotiation.IsActive)
                return null;

            return negotiation;
        }

        /// <summary>
        /// Return negotiation with given negotiation token
        /// </summary>
        /// <param name="token">Negotiation token given during creating negotiation</param>
        /// <returns></returns>
        public Negotiation GetNegotiationByToken(string token)
        {
            Negotiation negotiation = _context.Negotiations.Where(n => n.NegotiationToken == token).FirstOrDefault();

            if (negotiation == null)
                return null;

            if (!negotiation.IsActive)
                return null;

            return negotiation;
        }

        /// <summary>
        /// Return all negotiations that is still active
        /// </summary>
        /// <returns></returns>
        public IQueryable<Negotiation> GetNegotiations()
        {
            IQueryable<Negotiation> negotiations = _context.Negotiations.Where(n=>n.IsActive == true);

            return negotiations;
        }

        /// <summary>
        /// Return negotiation with specified status
        /// </summary>
        /// <param name="status">Status of negotiation</param>
        /// <returns></returns>
        public IQueryable<Negotiation> GetNegotiationsWithStatus(string status)
        {
            IQueryable<Negotiation> negotiations = _context.Negotiations.Where(n => n.Status == status);

            return negotiations;
        }

        /// <summary>
        /// Check if negotiation token is used in any other place
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool isTokenTaken(string token)
        {
            bool result = _context.Negotiations.Any(n => n.NegotiationToken == token);

            return result;
        }

        /// <summary>
        /// Update data about negotiation
        /// </summary>
        /// <param name="negotiation">New data</param>
        /// <returns>Negotiation ID</returns>
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
