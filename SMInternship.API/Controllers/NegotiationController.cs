using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMInternship.Application.DTO.Negotiations;
using SMInternship.Application.Interfaces;

namespace SMInternship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationController : ControllerBase
    {
        private readonly INegotiationService _negotiationService;

        public NegotiationController(INegotiationService negotiationService)
        {
            _negotiationService = negotiationService;
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody]NewNegotiationDTO dto)
        {
            string result = _negotiationService.AddNegotiation(dto);

            if (result.EndsWith('.'))
                return BadRequest(result);

            return Ok(result);

        }

        [Authorize]
        [HttpGet("Get/{id}")]
        public IActionResult Get(int id)
        {
            var result = _negotiationService.GetNegotiationDetails(id);

            if (result == null)
                return NotFound("There is no negotiation with this ID.");
            return Ok(result);
        }

        [HttpGet("GetByToken/{token}")]
        public IActionResult GetByToken(string token)
        {
            var result = _negotiationService.GetNegotiationDetails(token);

            if (result == null)
                return NotFound("There is no negotiation with this token.");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("GetList")]
        public IActionResult GetList([FromBody]NegotiationSearchInfo info)
        {
            var result = _negotiationService.GetNegotiations(info);

            if (result == null)
                return BadRequest("Send correct info about page and size.");
            if (result.NegotiationList.Count == 0)
                return NotFound("There is no products on this page.");

            return Ok(result);

        }

        [Authorize]
        [HttpPut("ResponseToOffer")]
        public IActionResult ResponseToOffer([FromBody]ResponseDTO dto)
        {

            var result = _negotiationService.ResponseToOffer(dto);

            switch (result)
            {
                case -1:
                    return BadRequest("Send response to offer.");
                case -2:
                    return BadRequest("ID cannot be lower than 1");
                case -3:
                    return NotFound($"There is no negotiation with ID: {dto.ID}");
                case -4:
                    return BadRequest("Already responsed to this negotiation.");
                default:
                    return Ok(result);
            }    
        }

        [HttpPut("NewOffer")]
        public IActionResult NewOffer([FromBody]NewOfferDTO dto)
        {
            var result = _negotiationService.SendNewOffer(dto);

            switch (result)
            {
                case -1:
                    return BadRequest("Send response to offer.");
                case -2:
                    return BadRequest("Token cannot be empty.");
                case -3:
                    return BadRequest("Price cannot be lower than 0.0.");
                case -4:
                    return NotFound($"There is no negotiation with token: {dto.Token}.");
                case -5:
                    return BadRequest("Negotiation has not been rejected yet.");
                default:
                    return Ok(result);
            }
        }

    }
}
