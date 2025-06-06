using Azure;
using Moq;
using Newtonsoft.Json.Linq;
using SMInternship.Application.DTO.Negotiations;
using SMInternship.Application.Services;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Tests.Services
{
    public class NegotiationServiceTests
    {
        [Fact]
        public void AddNegotiation_NoData_ReturnNoDataInfo()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            NewNegotiationDTO data = null;

            //Act
            var result = negotiationService.AddNegotiation(data);

            //Assert
            Assert.Equal("No data sent.", result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.0)]
        public void AddNegotiation_PriceBelowEqualZero_ReturnNoDataInfo(double price)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewNegotiationDTO()
            {
                Price = price,
                ProductID = 4
            };

            //Act
            var result = negotiationService.AddNegotiation(data);

            //Assert
            Assert.Equal("Bad price.", result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }


        [Fact]
        public void AddNegotiation_ProductDoesntExist_ReturnNoDataInfo()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewNegotiationDTO()
            {
                Price = 10.4,
                ProductID = 4
            };

            mockProd.Setup(p => p.GetProduct(data.ProductID)).Returns((Product?)null);

            //Act
            var result = negotiationService.AddNegotiation(data);

            //Assert
            Assert.Equal("Product doesn't exist.", result);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Once);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddNegotiation_CreateValidNegotiation_ReturnNoDataInfo()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);
            var callCount = 0;

            var data = new NewNegotiationDTO()
            {
                Price = 10.4,
                ProductID = 4
            };

            mockProd.Setup(p => p.GetProduct(data.ProductID)).Returns(new Product());
            mockNeg.Setup(n => n.isTokenTaken(It.IsAny<string>()))
                .Returns(() =>
                {
                    callCount++;
                    return callCount <= 3;
                });

            //Act
            var result = negotiationService.AddNegotiation(data);

            //Assert
            Assert.True(result.Length == 12);
            Assert.False(result.EndsWith('.'));
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Once);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.isTokenTaken(It.IsAny<string>()), Times.Exactly(4));  //4th time result is false
            mockNeg.Verify(n => n.AddNegotiation(It.IsAny<Negotiation>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }


        [Theory]
        [InlineData(-5)]
        [InlineData(null)]
        public void GetNegotiation_WrongIndex_ReturnNull(int id)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            //Act
            var result = negotiationService.GetNegotiationDetails(id);

            //Assert
            Assert.Null(result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetNegotiation_WrongToken_ReturnNull(string token)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            //Act
            var result = negotiationService.GetNegotiationDetails(token);

            //Assert
            Assert.Null(result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetNegotiation_NegotiationNotFound_ReturnNull(int choice)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            int id = 5;
            string token = "NegotiationToken";

            mockNeg.Setup(n => n.GetNegotiation(id)).Returns((Negotiation?)null);
            mockNeg.Setup(n => n.GetNegotiationByToken(token)).Returns((Negotiation?)null);
            //Act
            var result = new NegotiationDetailsDTO();
            if (choice == 1)
                result = negotiationService.GetNegotiationDetails(id);
            else
                result = negotiationService.GetNegotiationDetails(token);


            //Assert
            Assert.Null(result);
            mockProd.VerifyNoOtherCalls();
            if (choice == 1)
                mockNeg.Verify(n => n.GetNegotiation(It.IsAny<int>()), Times.Once);
            else
                mockNeg.Verify(n => n.GetNegotiationByToken(It.IsAny<string>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetNegotiation_ValidRequest_ReturnNegotiationDetails(int choice)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            int id = 5;
            string token = "NegotiationToken";

            var neg = new Negotiation()
            {
                ID = id,
                AttempCounter = 0,
                IsActive = true,
                LastAttemp = DateTime.Today,
                NegotiationToken = "NegotiationToken",
                Price = 10.5,
                ProductID = 18,
                Status = NegotiationStatus.Pending
            };

            var prod = new Product()
            {
                ID = 18,
                Name = "Product name"
            };

            mockNeg.Setup(n => n.GetNegotiation(id)).Returns(neg);
            mockNeg.Setup(n => n.GetNegotiationByToken(token)).Returns(neg);
            mockProd.Setup(p => p.GetProduct(neg.ProductID)).Returns(prod);

            //Act
            var result = new NegotiationDetailsDTO();
            if (choice == 1)
            {
                result = negotiationService.GetNegotiationDetails(id);
            }
            else
            {
                result = negotiationService.GetNegotiationDetails(token);

            }

            //Assert
            Assert.Equal(5, result.ID);
            Assert.Equal(0, result.AttempCounter);
            Assert.Equal(10.5, result.Price);
            Assert.Equal(18, result.ProductID);
            Assert.Equal(NegotiationStatus.Pending, result.Status);
            Assert.Equal(DateTime.Today, result.LastAttemp);
            Assert.Equal("NegotiationToken", result.NegotiationToken);
            Assert.Equal("Product name", result.ProductName);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Once);
            mockProd.VerifyNoOtherCalls();
            if (choice == 1)
                mockNeg.Verify(n => n.GetNegotiation(It.IsAny<int>()), Times.Once);
            else
                mockNeg.Verify(n => n.GetNegotiationByToken(It.IsAny<string>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendNewOffer_NoDataSent_ReturnMinusOne()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            NewOfferDTO data = null;

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(-1, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SendNewOffer_EmptyTokenSent_ReturnMinusTwo(string token)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewOfferDTO()
            {
                Price = 10.0,
                Token = token
            };

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(-2, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }


        [Theory]
        [InlineData(-4.0)]
        [InlineData(0.0)]
        public void SendNewOffer_PriceBelowEqualsZero_ReturnMinusThree(double price)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewOfferDTO()
            {
                Token = "Token",
                Price = price
            };

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(-3, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendNewOffer_NoNegotiationFound_ReturnMinusFour()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewOfferDTO()
            {
                Price = 10.0,
                Token = "token"
            };

            mockNeg.Setup(n => n.GetNegotiationByToken(data.Token)).Returns((Negotiation?)null);

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(-4, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiationByToken(It.IsAny<string>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }


        [Theory]
        [InlineData(NegotiationStatus.Canceled)]
        [InlineData(NegotiationStatus.Accepted)]
        [InlineData(NegotiationStatus.Pending)]
        public void SendNewOffer_WrongStatus_ReturnMinusFive(NegotiationStatus status)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewOfferDTO()
            {
                Price = 10.0,
                Token = "token"
            };

            var neg = new Negotiation()
            {
                NegotiationToken = data.Token,
                Status = status
            };

            mockNeg.Setup(n => n.GetNegotiationByToken(data.Token)).Returns(neg);

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(-5, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiationByToken(It.IsAny<string>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendNewOffer_ValidOffer_ReturnNegotiationID()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NewOfferDTO()
            {
                Price = 10.0,
                Token = "token"
            };

            var neg = new Negotiation()
            {
                ID = 10,
                NegotiationToken = data.Token,
                Status = NegotiationStatus.Rejected
            };

            mockNeg.Setup(n => n.GetNegotiationByToken(data.Token)).Returns(neg);

            //Act
            var result = negotiationService.SendNewOffer(data);

            //Assert
            Assert.Equal(10, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiationByToken(It.IsAny<string>()), Times.Once);
            mockNeg.Verify(n => n.UpdateNegotiation(It.IsAny<Negotiation>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void ResponseToOffer_NoDataSent_ReturnMinusOne()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            ResponseDTO data = null;

            //Act
            var result = negotiationService.ResponseToOffer(data);

            //Asssert
            Assert.Equal(-1, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void ResponseToOffer_IDBelowOne_ReturnMinusTwo()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new ResponseDTO()
            {
                ID = -3,
                Status = NegotiationStatus.Rejected
            };

            //Act
            var result = negotiationService.ResponseToOffer(data);

            //Asssert
            Assert.Equal(-2, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void ResponseToOffer_NegotiationNotFound_ReturnMinusThree()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new ResponseDTO()
            {
                ID = 5,
                Status = NegotiationStatus.Rejected
            };

            mockNeg.Setup(n => n.GetNegotiation(data.ID)).Returns((Negotiation?)null);

            //Act
            var result = negotiationService.ResponseToOffer(data);

            //Asssert
            Assert.Equal(-3, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiation(It.IsAny<int>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(NegotiationStatus.Canceled)]
        [InlineData(NegotiationStatus.Accepted)]
        [InlineData(NegotiationStatus.Rejected)]
        public void ResponseToOffer_WrongNegotiationStatus_ReturnMinusFour(NegotiationStatus status)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new ResponseDTO()
            {
                ID = 5,
                Status = NegotiationStatus.Accepted
            };

            var negotiation = new Negotiation()
            {
                ID = data.ID,
                Status = status
            };

            mockNeg.Setup(n => n.GetNegotiation(data.ID)).Returns(negotiation);

            //Act
            var result = negotiationService.ResponseToOffer(data);

            //Asssert
            Assert.Equal(-4, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiation(It.IsAny<int>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }


        [Fact]
        public void ResponseToOffer_ValidResponse_ReturnNegotiationID()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new ResponseDTO()
            {
                ID = 5,
                Status = NegotiationStatus.Accepted
            };

            var negotiation = new Negotiation()
            {
                ID = data.ID,
                Status = NegotiationStatus.Pending
            };

            mockNeg.Setup(n => n.GetNegotiation(data.ID)).Returns(negotiation);

            //Act
            var result = negotiationService.ResponseToOffer(data);

            //Asssert
            Assert.Equal(5, result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiation(It.IsAny<int>()), Times.Once);
            mockNeg.Verify(n => n.UpdateNegotiation(It.IsAny<Negotiation>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetNegotiations_NoDataSent_ReturnNull()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            NegotiationSearchInfo data = null;

            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.Null(result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(-3,4)]
        [InlineData(3,-10)]
        [InlineData(-3,-5)]
        public void GetNegotiations_WrongPageSettings_ReturnNull(int page, int pageSize)
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                Page = page,
                PageSize = pageSize
            };

            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.Null(result);
            mockProd.VerifyNoOtherCalls();
            mockNeg.VerifyNoOtherCalls();
        }


        [Fact]
        public void GetNegotiations_NoSearchingValues_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = null,
                SearchingStatus = null,
                Page = 1,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            { 
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Rejected,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Pending,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Canceled,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var products = new List<Product>()
            {
                new Product(){ ID = 5, Name = "First"},
                new Product(){ ID = 15, Name = "Second"}
            };


            mockNeg.Setup(n => n.GetNegotiations()).Returns(negotiations);
            mockProd.Setup(p => p.GetProduct(5)).Returns(products[0]);
            mockProd.Setup(p => p.GetProduct(15)).Returns(products[1]);
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.Pages);
            Assert.Null(result.SearchingProduct);
            Assert.Null(result.SearchingStatus);
            Assert.Equal(5, result.Count);
            Assert.Equal(1, result.NegotiationList[0].ID);
            Assert.Equal(5, result.NegotiationList[0].ProductID);
            Assert.Equal(NegotiationStatus.Accepted, result.NegotiationList[0].Status);
            Assert.Equal(DateTime.Today, result.NegotiationList[0].LastAttemp);
            Assert.Equal(2, result.NegotiationList[1].ID);
            Assert.Equal(15, result.NegotiationList[1].ProductID);
            Assert.Equal(NegotiationStatus.Accepted, result.NegotiationList[1].Status);
            Assert.Equal(DateTime.Today, result.NegotiationList[1].LastAttemp);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Exactly(2));            
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiations(), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetNegotiations_SecondPageItems_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = null,
                SearchingStatus = null,
                Page = 2,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            {
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Rejected,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Pending,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Canceled,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var products = new List<Product>()
            {
                new Product(){ ID = 2, Name = "Third"},
                new Product(){ ID = 7, Name = "Fotrh"}
            };


            mockNeg.Setup(n => n.GetNegotiations()).Returns(negotiations);
            mockProd.Setup(p => p.GetProduct(2)).Returns(products[0]);
            mockProd.Setup(p => p.GetProduct(7)).Returns(products[1]);
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.Pages);
            Assert.Null(result.SearchingProduct);
            Assert.Null(result.SearchingStatus);
            Assert.Equal(5, result.Count);
            Assert.Equal(3, result.NegotiationList[0].ID);
            Assert.Equal(4, result.NegotiationList[1].ID);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Exactly(2)); 
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiations(), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }


        [Fact]
        public void GetNegotiations_LastPageItems_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = null,
                SearchingStatus = null,
                Page = 3,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            {
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Rejected,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Pending,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Canceled,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var product = new Product() { ID = 5, Name = "Last" };


            mockNeg.Setup(n => n.GetNegotiations()).Returns(negotiations);
            mockProd.Setup(p => p.GetProduct(5)).Returns(product);
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.Pages);
            Assert.Single(result.NegotiationList);
            Assert.Null(result.SearchingProduct);
            Assert.Null(result.SearchingStatus);
            Assert.Equal(5, result.Count);
            Assert.Equal(5, result.NegotiationList[0].ID);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Once); 
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiations(), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetNegotiations_OnlyStatusSearching_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = null,
                SearchingStatus = NegotiationStatus.Accepted,
                Page = 1,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            {
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var product = new Product() { ID = 5, Name = "Last" };


            mockNeg.Setup(n => n.GetNegotiationsWithStatus(data.SearchingStatus.Value)).Returns(negotiations);
            mockProd.Setup(p => p.GetProduct(It.IsAny<int>())).Returns(product);
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.Pages);
            Assert.Equal(2, result.NegotiationList.Count);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Exactly(2));
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiationsWithStatus(It.IsAny<NegotiationStatus>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetNegotiations_OnlyProductSearching_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = "Test",
                SearchingStatus = null,
                Page = 1,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            {
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var products = new List<Product>()
            {
                new Product(){ID = 5, Name = "Test1"},
                new Product(){ID = 6, Name = "Test2" },
                new Product(){ID = 7, Name = "Test3"},
                new Product(){ID = 8, Name = "Test4"},
            }.AsQueryable();

            mockNeg.Setup(n => n.GetNegotiations()).Returns(negotiations);
            mockProd.Setup(p => p.GetProductsByName(data.SearchingProduct)).Returns(products);
            mockProd.Setup(p => p.GetProduct(It.IsAny<int>())).Returns(new Product() { Name = "Item"});
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(2, result.Pages);
            Assert.Equal(2, result.NegotiationList.Count);
            mockProd.Verify(p => p.GetProductsByName(It.IsAny<String>()), Times.Once);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Exactly(2));
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiations(), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }


        [Fact]
        public void GetNegotiations_BothSearching_ReturnNegotiationList()
        {
            //Arrange
            var mockProd = new Mock<IProductRepository>();
            var mockNeg = new Mock<INegotiationRepository>();
            var negotiationService = new NegotiationService(mockNeg.Object, mockProd.Object);

            var data = new NegotiationSearchInfo()
            {
                SearchingProduct = "Test",
                SearchingStatus = NegotiationStatus.Accepted,
                Page = 1,
                PageSize = 2
            };

            var negotiations = new List<Negotiation>()
            {
                new Negotiation()
                {
                    ID = 1,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 2,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 15,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 3,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 2,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 4,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 7,
                    LastAttemp = DateTime.Today,
                },
                new Negotiation()
                {
                    ID = 5,
                    Status = NegotiationStatus.Accepted,
                    ProductID = 5,
                    LastAttemp = DateTime.Today,
                },
            }.AsQueryable();

            var products = new List<Product>()
            {
                new Product(){ID = 5, Name = "Test1"},
                new Product(){ID = 6, Name = "Test2" },
                new Product(){ID = 7, Name = "Test3"},
                new Product(){ID = 8, Name = "Test4"},
            }.AsQueryable();

            mockNeg.Setup(n => n.GetNegotiationsWithStatus(It.IsAny<NegotiationStatus>())).Returns(negotiations);
            mockProd.Setup(p => p.GetProductsByName(data.SearchingProduct)).Returns(products);
            mockProd.Setup(p => p.GetProduct(It.IsAny<int>())).Returns(new Product() { Name = "Item" });
            //Act
            var result = negotiationService.GetNegotiations(data);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ActualPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(2, result.Pages);
            Assert.Equal(2, result.NegotiationList.Count);
            mockProd.Verify(p => p.GetProductsByName(It.IsAny<String>()), Times.Once);
            mockProd.Verify(p => p.GetProduct(It.IsAny<int>()), Times.Exactly(2));
            mockProd.VerifyNoOtherCalls();
            mockNeg.Verify(n => n.GetNegotiationsWithStatus(It.IsAny<NegotiationStatus>()), Times.Once);
            mockNeg.VerifyNoOtherCalls();
        }

    }
}


