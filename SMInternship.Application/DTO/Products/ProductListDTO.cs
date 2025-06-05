using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Products
{
    public class ProductListDTO
    {
        public List<ProductItemDTO> ProductList { get; set; }
        public int Count { get; set; }
        public string? SearchingName { get; set; }

        public int ActualPage { get; set; }
        public int Pages { get; set; }
        public int PageSize { get; set; }


        public ProductListDTO()
        {
            ProductList = new List<ProductItemDTO>();
        }
    }
}
