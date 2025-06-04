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
