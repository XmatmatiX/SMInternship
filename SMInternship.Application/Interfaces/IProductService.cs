using SMInternship.Application.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Interfaces
{
    public interface IProductService
    {
        ProductDetailsDTO GetProduct(int id);
        ShowProductListDTO GetProductList(int page, int pageSize, string? searchingName);
        int AddProduct(NewProductDTO dto);
        int UpdateProduct(ProductDetailsDTO dto);
    }
}
