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
            var result = _productService.GetProduct(id);

            if (result == null)
                return NotFound($"Not found item with ID: {id}");

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Add")]
        public IActionResult Add([FromBody]NewProductDTO? dto)
        {
            var result = _productService.AddProduct(dto);

            if (result == -3)
                return BadRequest("No data has been sent.");

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

            var result = _productService.UpdateProduct(dto);

            if (result == -2)
                return BadRequest("No data has been sent.");

            if (result == -1)
                return BadRequest("This name is already taken.");

            return Ok(result);

        }

        [HttpGet("GetList")]
        public IActionResult GetList([FromBody] ProductSearchInfo info)
        {
            var result = _productService.GetProductList(info);

            if (result == null)
                return BadRequest("Bad page size or number.");

            if (result.ProductList.Count == 0)
                return NotFound("There is no products on this page.");

            return Ok(result);
        }

    }
}
