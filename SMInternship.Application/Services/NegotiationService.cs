using SMInternship.Application.DTO.Negotiations;
using SMInternship.Application.Interfaces;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Services
{
    public class NegotiationService : INegotiationService
    {
        private readonly INegotiationRepository _negotiationRepository;
        private readonly IProductRepository _productRepository;

        public NegotiationService(INegotiationRepository negotiationRepository, IProductRepository productRepository)
        {
            _negotiationRepository = negotiationRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Create new negotiation about specified product
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Negotiation token used by customert to check negotiation status</returns>
        public string AddNegotiation(NewNegotiationDTO dto)
        {
            if (dto.Price <= 0.0)
                return "Bad price.";

            if (_productRepository.GetProduct(dto.ProductID) == null)
                return "Product doesn't exist.";
            string token = string.Empty;
            do
            {
                token = createToken();
            } while (_negotiationRepository.isTokenTaken(token));

            Negotiation negotiation = new Negotiation()
            {
                AttempCounter = 0,
                IsActive = true,
                LastAttemp = DateTime.UtcNow,
                Price = dto.Price,
                ProductID = dto.ProductID,
                Status = NegotiationStatus.Pending,
                NegotiationToken = token
            };

            _negotiationRepository.AddNegotiation(negotiation);

            return token;
        }

        /// <summary>
        /// Get details about negotiation with specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Details about negotiation</returns>
        public NegotiationDetailsDTO GetNegotiationDetails(int id)
        {
            var negotiation = _negotiationRepository.GetNegotiation(id);

            if (negotiation == null)
                return null;

            NegotiationDetailsDTO result = new NegotiationDetailsDTO()
            {
                ID = negotiation.ID,
                AttempCounter = negotiation.AttempCounter,
                Price = negotiation.Price,
                ProductID = negotiation.ProductID,
                Status = negotiation.Status,
                LastAttemp = negotiation.LastAttemp,
                NegotiationToken = negotiation.NegotiationToken
            };

            string productName = _productRepository.GetProduct(result.ProductID).Name;
            result.ProductName = productName;

            return result;
        }

        /// <summary>
        /// Get details about negotiation with specified negotiation token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Details about negotiation</returns>
        public NegotiationDetailsDTO GetNegotiationDetails(string token)
        {
            var negotiation = _negotiationRepository.GetNegotiationByToken(token);

            if (negotiation == null)
                return null;

            NegotiationDetailsDTO result = new NegotiationDetailsDTO()
            {
                ID = negotiation.ID,
                AttempCounter = negotiation.AttempCounter,
                Price = negotiation.Price,
                ProductID = negotiation.ProductID,
                Status = negotiation.Status,
                LastAttemp = negotiation.LastAttemp,
                NegotiationToken = negotiation.NegotiationToken
            };

            string productName = _productRepository.GetProduct(result.ProductID).Name;
            result.ProductName = productName;

            return result;
        }

        /// <summary>
        /// Get list of negotiation including pagination and searching options
        /// </summary>
        /// <param name="info"></param>
        /// <returns>List of negotiation with searching options</returns>
        public NegotiationListDTO GetNegotiations(NegotiationSearchInfo info)
        {
            IQueryable<Negotiation> negotiations;
            NegotiationListDTO result = new NegotiationListDTO();

            if (info.SearchingStatus == null)
            {
                negotiations = _negotiationRepository.GetNegotiations();
            }
            else
            {
                negotiations = _negotiationRepository.GetNegotiationsWithStatus(info.SearchingStatus.Value);
            }

            if (info.SearchingProduct != null)
            {
                List<int> productIDs = _productRepository.GetProductsByName(info.SearchingProduct)
                    .Select(p => p.ID)
                    .ToList();

                negotiations = negotiations.Where(n => productIDs.Contains(n.ProductID));
            }

            result.SearchingStatus = info.SearchingStatus;
            result.SearchingProduct = info.SearchingProduct;
            result.ActualPage = info.Page;
            result.PageSize = info.PageSize;
            result.Count = negotiations.Count();
            result.Pages = 1 + ((result.Count - 1) / info.PageSize);

            var showNegotiations = negotiations.Skip(info.PageSize * (info.Page - 1)).Take(info.PageSize).ToList();

            foreach (var negotiation in showNegotiations)
            {
                NegotiationItemDTO neg = new NegotiationItemDTO()
                {
                    ID = negotiation.ID,
                    LastAttemp = negotiation.LastAttemp,
                    ProductID = negotiation.ProductID,
                    ProductName = _productRepository.GetProduct(negotiation.ProductID).Name,
                    Status = negotiation.Status
                };

                result.NegotiationList.Add(neg);
            }

            return result;

        }

        /// <summary>
        /// Make response to client offer
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Negotiation ID</returns>
        public int ResponseToOffer(ResponseDTO dto)
        {
            var negotiation = _negotiationRepository.GetNegotiation(dto.ID);
            
            if (negotiation == null)
                return -1;
            else if (negotiation.Status != NegotiationStatus.Pending)
                return -2;

            negotiation.LastAttemp = DateTime.UtcNow;
            negotiation.AttempCounter++;
            if (negotiation.AttempCounter > 3)
                negotiation.Status = NegotiationStatus.Canceled;
            else
                negotiation.Status = dto.Status;


            _negotiationRepository.UpdateNegotiation(negotiation);
            return negotiation.ID;

        }

        /// <summary>
        /// Send new offer to employee
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Negotiation ID</returns>
        public int SendNewOffer(NewOfferDTO dto)
        {
            var negotiation = _negotiationRepository.GetNegotiationByToken(dto.Token);

            if (negotiation == null)
                return -1;
            else if (negotiation.Status != NegotiationStatus.Rejected)
                return -2;
            else if (negotiation.Price <= 0.0)
                return -3;

            negotiation.LastAttemp = DateTime.UtcNow;
            negotiation.Price = dto.Price;
            negotiation.Status = NegotiationStatus.Pending;

            _negotiationRepository.UpdateNegotiation(negotiation);
            return negotiation.ID;
        }


        /// <summary>
        /// Creating token for negotiation that will be used by clients
        /// </summary>
        /// <returns></returns>
        private string createToken()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
