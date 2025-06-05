using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMInternship.Application.DTO.Products;
using SMInternship.Application.Interfaces;

namespace SMInternship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("Get/{id}")]
        public IActionResult Get(int id)
        {
            if (id < 0)
                return BadRequest("ID cannot be lower than 0.");

            var result = _productService.GetProduct(id);

            if (result == null)
                return NotFound($"Not found item with ID: {id}");

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Add")]
        public IActionResult Add([FromBody]NewProductDTO? dto)
        {
            if (dto == null)
                return BadRequest("Sent object is empty.");

            var result = _productService.AddProduct(dto);

            if (result == -2)
                return BadRequest("This name is already taken.");

            if (result == -1)
                return BadRequest("Something is wrong with sent object");
            return Ok(result);
        }

        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromBody]ProductDetailsDTO dto)
        {
            if (dto == null)
                return BadRequest("Sent object is empty.");

            var result = _productService.UpdateProduct(dto);

            if (result == -1)
                return BadRequest("This name is already taken.");

            return Ok(result);

        }

        [HttpGet("GetList")]
        public IActionResult GetList([FromBody] ProductSearchInfo info)
        {
            if (info == null)
                return BadRequest("Send info about page and size.");

            if (info.PageSize < 1 || info.Page < 1)
                return BadRequest("Bad page size or number.");

            var result = _productService.GetProductList(info);

            if (result.ProductList.Count == 0)
                return NotFound("There is no products on this page.");

            return Ok(result);
        }

    }
}
