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

        /// <summary>
        /// Add new product to database
        /// </summary>
        /// <param name="product">new product data</param>
        /// <returns>ID of new product</returns>
        public int AddProduct(Product product)
        {
            if (product == null)
            {
                return -1;
            }

            Product obj = _context.Products.Add(product).Entity;

            _context.SaveChanges();

            return obj.ID;
        }

        /// <summary>
        /// Get product with specified ID
        /// </summary>
        /// <param name="productId">ID of searched product</param>
        /// <returns></returns>
        public Product GetProduct(int productId)
        {
            Product product = _context.Products.FirstOrDefault(p => p.ID == productId);

            if (product == null)
                return null;

            if (!product.IsActive)
                return null;

            return product;
        }

        /// <summary>
        /// Returns list of products which weren't soft deleted
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetProducts()
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsActive == true);

            return products;
        }

        /// <summary>
        /// Get products which contain specified string in name
        /// </summary>
        /// <param name="productName">Searching phrase</param>
        /// <returns></returns>
        public IQueryable<Product> GetProductsByName(string productName)
        {
            IQueryable<Product> products = _context.Products
                .Where(p => p.Name.Contains(productName) && p.IsActive == true);

            return products;
        }

        /// <summary>
        /// Checking if name is already taken by existing product (not soft deleted)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsNameTaken(string name)
        {
            var result = _context.Products.Any(p => p.Name == name && p.IsActive == true);

            return result;
        }

        /// <summary>
        /// Updates data of product basing on ID
        /// </summary>
        /// <param name="product">New product data</param>
        /// <returns>ID of product</returns>
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
