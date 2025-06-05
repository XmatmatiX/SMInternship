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
            if (dto == null)
                return BadRequest("No data was sent.");

            string result = _negotiationService.AddNegotiation(dto);

            if (result.EndsWith('.'))
                return BadRequest(result);

            return Ok(result);

        }

        [HttpGet("Get/{id}")]
        public IActionResult Get(int id)
        {
            if (id < 0)
                return BadRequest("ID cannot be lower than 0.");

            var result = _negotiationService.GetNegotiationDetails(id);

            if (result == null)
                return NotFound("There is no negotiation with this ID.");
            return Ok(result);
        }

        [HttpGet("GetByToken/{token}")]
        public IActionResult GetByToken(string token)
        {
            if (token == null)
                return BadRequest("There is no token in request.");

            var result = _negotiationService.GetNegotiationDetails(token);

            if (result == null)
                return NotFound("There is no negotiation with this token.");
            return Ok(result);
        }

        [HttpGet("GetList")]
        public IActionResult GetList([FromBody]NegotiationSearchInfo info)
        {
            if (info == null)
                return BadRequest("Send info about page and size.");

            if (info.PageSize < 1 || info.Page < 1)
                return BadRequest("Bad page size or number.");

            var result = _negotiationService.GetNegotiations(info);

            if (result.NegotiationList.Count == 0)
                return NotFound("There is no products on this page.");

            return Ok(result);

        }

        [HttpPut("ResponseToOffer")]
        public IActionResult ResponseToOffer([FromBody]ResponseDTO dto)
        {
            if (dto == null)
                return BadRequest("Send response to offer.");

            if (dto.ID < 0)
                return BadRequest("ID cannot be lower than 0");

            var result = _negotiationService.ResponseToOffer(dto);

            if (result == -1)
                return NotFound($"There is no negotiation with ID: {dto.ID}");
            else if (result == -2)
                return BadRequest("Already responsed to this negotiation.");

            return Ok(result);
        }

        [HttpPut("NewOffer")]
        public IActionResult NewOffer([FromBody]NewOfferDTO dto)
        {
            if(dto == null)
                return BadRequest("Send response to offer.");

            if(dto.Token == null)
                return BadRequest("Token cannot be null.");

            var result = _negotiationService.SendNewOffer(dto);

            switch (result)
            {
                case -1:
                    return NotFound($"There is no negotiation with token: {dto.Token}.");
                case -2:
                    return BadRequest("Negotiation has not been rejected yet.");
                case -3:
                    return BadRequest("Price cannot be lower than 0.0.");
                default:
                    return Ok(result);
            }
        }

    }
}
