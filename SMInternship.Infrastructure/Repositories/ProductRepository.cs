using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;

        public ProductRepository(Context context)
        {
            _context = context;
        }

        public int AddProduct(Product product)
        {
            if (product == null)
            {
                return -1;
            }

            Product obj = _context.Products.Add(product).Entity;


            return obj.ID;
        }

        public Product GetProduct(int productId)
        {
            Product product = _context.Products.FirstOrDefault(p => p.ID == productId);

            return product;
        }

        public IQueryable<Product> GetProducts()
        {
            IQueryable<Product> products = _context.Products;

            return products;
        }

        public IQueryable<Product> GetProductsByName(string productName)
        {
            IQueryable<Product> products = _context.Products.Where(p => p.Name == productName);

            return products;
        }

        public int UpdateProduct(Product product)
        {
            _context.Attach(product);
            _context.Entry(product).Property(p => p.Name).IsModified = true;
            _context.Entry(product).Property(p => p.Description).IsModified = true;
            _context.Entry(product).Property(p => p.IsActive).IsModified = true;

            _context.SaveChanges();

            return product.ID;
        }
    }
}
