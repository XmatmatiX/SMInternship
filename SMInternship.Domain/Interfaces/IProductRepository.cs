using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Interfaces
{
    public interface IProductRepository
    {
        int AddProduct(Product product);
        Product GetProduct(int productId);
        IQueryable<Product> GetProductsByName(string productName);
        IQueryable<Product> GetProducts();

        int UpdateProduct(Product product);
    }
}
