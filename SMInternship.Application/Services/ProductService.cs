using SMInternship.Application.DTO.Products;
using SMInternship.Application.Interfaces;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Add new product to database
        /// </summary>
        /// <param name="dto">New product data</param>
        /// <returns>Product ID. -1 in case when name is already taken</returns>
        public int AddProduct(NewProductDTO dto)
        {
            if (_productRepository.IsNameTaken(dto.Name))
            {
                return -1;
            }

            Product product = new Product()
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };
            
            var result = _productRepository.AddProduct(product);

            return result;
        }


        /// <summary>
        /// Get product with specified ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product model. Null when product was not found</returns>
        public ProductDetailsDTO GetProduct(int id)
        {
            Product product = _productRepository.GetProduct(id);
            
            if (product == null)
                return null;

            ProductDetailsDTO dto = new ProductDetailsDTO(){
                ID = product.ID,
                Name = product.Name,
                Description = product.Description
            };

            return dto;
        }


        /// <summary>
        /// Get list of products with pagination and searching by name.
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of products on one page</param>
        /// <param name="searchingName">Searching phrase</param>
        /// <returns></returns>
        public ShowProductListDTO GetProductList(int page, int pageSize, string? searchingName)
        {
            IQueryable<Product> products;
            ShowProductListDTO result = new ShowProductListDTO();
            if (searchingName == null)
                products = _productRepository.GetProducts();
            else
                products = _productRepository.GetProductsByName(searchingName);

            result.ActualPage = page;
            result.PageSize = pageSize;
            result.SearchingName = searchingName;
            result.Count = products.Count();
            result.Pages = 1 + ((result.Count - 1) / pageSize);

            var showProducts = products.Skip(pageSize * (page - 1)).Take(pageSize);

            foreach (var product in showProducts)
            {
                ProductDetailsDTO prod = new ProductDetailsDTO()
                {
                    ID = product.ID,
                    Name = product.Name,
                    Description = product.Description
                };

                result.ProductList.Add(prod);
            }

            return result;

        }

        /// <summary>
        /// Update data about product
        /// </summary>
        /// <param name="dto">Product new data</param>
        /// <returns></returns>
        public int UpdateProduct(ProductDetailsDTO dto)
        {
            if (_productRepository.IsNameTaken(dto.Name))
            {
                return -1;
            }

            Product product = new Product()
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description
            };

            int result = _productRepository.UpdateProduct(product);

            return result;
        }
    }
}
