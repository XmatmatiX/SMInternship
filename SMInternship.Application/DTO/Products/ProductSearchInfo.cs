using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Products
{
    public class ProductSearchInfo
    {
        public string? SearchingName { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
