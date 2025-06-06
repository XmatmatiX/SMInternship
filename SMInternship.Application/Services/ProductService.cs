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
        /// <returns>Product ID. Returns number below 0 in case of some error</returns>
        public int AddProduct(NewProductDTO dto)
        {
            if (dto == null)
                return -2;

            if (_productRepository.IsNameTaken(dto.Name))
            {
                return -3;
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
            if (id <= 0)
                return null;

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
        public ProductListDTO GetProductList(ProductSearchInfo info)
        {
            if (info == null)
                return null;

            if (info.Page < 1 || info.PageSize < 1)
                return null;

            IQueryable<Product> products;
            ProductListDTO result = new ProductListDTO();
            if (info.SearchingName == null || info.SearchingName == "")
                products = _productRepository.GetProducts();
            else
                products = _productRepository.GetProductsByName(info.SearchingName);

            result.ActualPage = info.Page;
            result.PageSize = info.PageSize;
            result.SearchingName = info.SearchingName;
            result.Count = products.Count();
            result.Pages = 1 + ((result.Count - 1) / info.PageSize);

            var showProducts = products.Skip(info.PageSize * (info.Page - 1)).Take(info.PageSize);

            foreach (var product in showProducts)
            {
                ProductItemDTO prod = new ProductItemDTO()
                {
                    ID = product.ID,
                    Name = product.Name
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
            if(dto == null) 
                return -2;

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
