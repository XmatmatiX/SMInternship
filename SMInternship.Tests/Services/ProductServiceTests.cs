using Azure;
using Moq;
using SMInternship.Application.DTO.Products;
using SMInternship.Application.Services;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Tests.Services
{
    public class ProductServiceTests
    {

        [Fact]
        public void AddProduct_NullObject_ReturnsMinusThree()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            NewProductDTO dto = null;

            //Act
            var result = productService.AddProduct(dto);

            //Assert
            Assert.Equal(-2, result);
            mockRepo.Verify(r => r.IsNameTaken(It.IsAny<string>()), Times.Never);
            mockRepo.Verify(r => r.AddProduct(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData("TestName", "TestDesc")]
        [InlineData("Bicycle", null)]
        public void AddProduct_NameIsTaken_ReturnsMinusTwo(string name, string? desc)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            var dto = new NewProductDTO { Name = name, Description = desc };

            mockRepo.Setup(r => r.IsNameTaken(name)).Returns(true);

            //Act
            var result = productService.AddProduct(dto);

            //Assert
            Assert.Equal(-3, result);
            mockRepo.Verify(r => r.AddProduct(It.IsAny<Product>()), Times.Never);

        }

        [Theory]
        [InlineData("TestName", "TestDesc")]
        [InlineData("Bicycle", null)]
        public void AddProduct_ValidProduct_ReturnProductID(string name, string? desc)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            var dto = new NewProductDTO { Name = name, Description = desc };
            var expectedResult = 42;

            mockRepo.Setup(r => r.IsNameTaken(dto.Name)).Returns(false);
            mockRepo.Setup(r => r.AddProduct(It.IsAny<Product>())).Returns(expectedResult);

            //Act

            var result = productService.AddProduct(dto);

            //Assert

            Assert.Equal(expectedResult, result);
            mockRepo.Verify(r => r.AddProduct(It.Is<Product>(p =>
                p.Name == dto.Name &&
                p.Description == dto.Description &&
                p.IsActive == true
            )), Times.Once);
        }


        [Theory]
        [InlineData(-4)]
        [InlineData(null)]
        public void GetProduct_WrongID_ReturnNull(int id)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);

            //Act
            var result = productService.GetProduct(id);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetProduct(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(823)]
        [InlineData(242)]
        public void GetProduct_IdNotExist_ReturnNull(int id)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);

            mockRepo.Setup(r=>r.GetProduct(id)).Returns((Product?)null);

            //Act
            var result = productService.GetProduct(id);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetProduct(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(823)]
        [InlineData(242)]
        public void GetProduct_IdExist_ReturnDetails(int id)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);

            Product product = new Product()
            {
                ID = id,
                IsActive = true,
                Name = "Name",
                Description = "Description"
            };

            mockRepo.Setup(r => r.GetProduct(id)).Returns(product);

            //Act
            var result = productService.GetProduct(id);

            //Assert
            Assert.Equal("Name", result.Name);
            Assert.Equal("Description", result.Description);
            Assert.Equal(id, result.ID);
            mockRepo.Verify(r => r.GetProduct(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetProductList_NullInfoData_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);

            //Act
            var result = productService.GetProductList(null);

            //Arrange
            Assert.Null(result);
            mockRepo.Verify(r => r.GetProducts(), Times.Never);
            mockRepo.Verify(r => r.GetProductsByName(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(-10,-10)]
        [InlineData(8, 0)]
        [InlineData(-4,20)]
        public void GetProductList_WrongPageSetup_ReturnNull(int page, int pageSize)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo()
            {
                Page = page,
                PageSize = pageSize
            };

            //Act
            var result = productService.GetProductList(info);

            //Arrange
            Assert.Null(result);
            mockRepo.Verify(r => r.GetProducts(), Times.AtMostOnce);
            mockRepo.Verify(r => r.GetProductsByName(It.IsAny<string>()), Times.AtMostOnce);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetProductList_EmptySearchingName_ReturnProductList(string? searching)
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo()
            {
                SearchingName = searching,
                Page = 1,
                PageSize = 5
            };

            var data = new List<Product>() 
            { 
                new Product() { ID = 1, Name = "one" },
                new Product() { ID = 2, Name = "two" },
                new Product() { ID = 3, Name = "three"}
            }.AsQueryable();

            mockRepo.Setup(r => r.GetProducts()).Returns(data);

            //Act
            var result = productService.GetProductList(info);

            //Arrange
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(3, result.Count);  
            Assert.Equal(searching, result.SearchingName);
            Assert.Equal("one", result.ProductList[0].Name);
            Assert.Equal("two", result.ProductList[1].Name);
            Assert.Equal("three", result.ProductList[2].Name);
            mockRepo.Verify(r => r.GetProducts(), Times.Once);
            mockRepo.Verify(r => r.GetProductsByName(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetProductList_WithSearchingName_ReturnProductList()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var productService = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo()
            {
                SearchingName = "searching",
                Page = 1,
                PageSize = 5
            };

            var data = new List<Product>()
            {
                new Product() { ID = 1, Name = "one" },
                new Product() { ID = 2, Name = "two" },
                new Product() { ID = 3, Name = "three"}
            }.AsQueryable();

            mockRepo.Setup(r => r.GetProductsByName("searching")).Returns(data);

            //Act
            var result = productService.GetProductList(info);

            //Arrange
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(3, result.Count);
            Assert.Equal("searching", result.SearchingName);
            Assert.Equal("one", result.ProductList[0].Name);
            Assert.Equal("two", result.ProductList[1].Name);
            Assert.Equal("three", result.ProductList[2].Name);
            mockRepo.Verify(r => r.GetProducts(), Times.Never);
            mockRepo.Verify(r => r.GetProductsByName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetProductList_Page2_ReturnsCorrectProducts()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo
            {
                Page = 2,
                PageSize = 2,
                SearchingName = null
            };

            var data = new List<Product>()
            {
                new Product() { ID = 1, Name = "one" },
                new Product() { ID = 2, Name = "two" },
                new Product() { ID = 3, Name = "three"},
                new Product() { ID = 4, Name = "four" },
                new Product() { ID = 5, Name = "five" }
            }.AsQueryable();

            mockRepo.Setup(r => r.GetProducts()).Returns(data);

            // Act
            var result = service.GetProductList(info);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(5, result.Count);
            Assert.Equal(3, result.Pages);
            Assert.Equal(2, result.ProductList.Count);
            Assert.Equal("three", result.ProductList[0].Name);
            Assert.Equal("four", result.ProductList[1].Name);
        }

        [Fact]
        public void GetProductList_OutOfRange_ReturnNullList()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo
            {
                Page = 4,
                PageSize = 2,
                SearchingName = null
            };

            var data = new List<Product>()
            {
                new Product() { ID = 1, Name = "one" },
                new Product() { ID = 2, Name = "two" },
                new Product() { ID = 3, Name = "three"},
                new Product() { ID = 4, Name = "four" },
                new Product() { ID = 5, Name = "five" }
            }.AsQueryable();

            mockRepo.Setup(r => r.GetProducts()).Returns(data);

            // Act
            var result = service.GetProductList(info);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.ProductList);
        }

        [Fact]
        public void GetProductList_LastPage_ReturnPartOfProducts()
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);
            var info = new ProductSearchInfo
            {
                Page = 3,
                PageSize = 2,
                SearchingName = null
            };

            var data = new List<Product>()
            {
                new Product() { ID = 1, Name = "one" },
                new Product() { ID = 2, Name = "two" },
                new Product() { ID = 3, Name = "three"},
                new Product() { ID = 4, Name = "four" },
                new Product() { ID = 5, Name = "five" }
            }.AsQueryable();

            mockRepo.Setup(r => r.GetProducts()).Returns(data);

            // Act
            var result = service.GetProductList(info);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.ProductList);
            Assert.Equal("five", result.ProductList[0].Name);
        }

        [Fact]
        public void UpdateProduct_NullInputData_ReturnMinusTwo()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            ProductDetailsDTO updateData = null;

            //Act
            var result = service.UpdateProduct(updateData);

            //Assert
            Assert.Equal(-2, result);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void UpdateProduct_NameExist_ReturnMinusOne()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            var updateData = new ProductDetailsDTO()
            {
                ID = 5,
                Description = "Description",
                Name = "Name"
            };

            mockRepo.Setup(r=>r.IsNameTaken("Name")).Returns(true);

            //Act
            var result = service.UpdateProduct(updateData);

            //Assert
            Assert.Equal(-1, result);
            mockRepo.Verify(r => r.IsNameTaken(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void UpdateProduct_ValidUpdate_ReturnProductID()
        {
            //Arrange
            var mockRepo = new Mock<IProductRepository>();
            var service = new ProductService(mockRepo.Object);

            var updateData = new ProductDetailsDTO()
            {
                ID = 5,
                Description = "Description",
                Name = "Name"
            };

            mockRepo.Setup(r => r.IsNameTaken("Name")).Returns(false);
            mockRepo.Setup(r=>r.UpdateProduct(It.IsAny<Product>())).Returns((Product p) => p.ID);
            //Act
            var result = service.UpdateProduct(updateData);

            //Assert
            Assert.Equal(5, result);
            mockRepo.Verify(r => r.IsNameTaken(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Once);
        }
    }
}
