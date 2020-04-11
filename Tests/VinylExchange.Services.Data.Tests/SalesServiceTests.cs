﻿namespace VinylExchange.Services.Data.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using VinylExchange.Data;
    using VinylExchange.Data.Common.Enumerations;
    using VinylExchange.Data.Models;
    using VinylExchange.Services.Data.MainServices.Addresses;
    using VinylExchange.Services.Data.MainServices.Releases;
    using VinylExchange.Services.Data.MainServices.Sales;
    using VinylExchange.Services.Data.MainServices.Users;
    using VinylExchange.Services.Data.Tests.TestFactories;
    using VinylExchange.Web.Models.InputModels.Sales;
    using VinylExchange.Web.Models.ResourceModels.Sales;

    using Xunit;

    using static VinylExchange.Common.Constants.NullReferenceExceptionsConstants;

    #endregion

    public class SalesServiceTests
    {
        private readonly VinylExchangeDbContext dbContext;

        private readonly ISalesService salesService;

        private readonly Mock<IAddressesEntityRetriever> addressesEntityRetrieverMock;

        private readonly Mock<IUsersEntityRetriever> usersEntityRetrieverMock;

        private readonly Mock<IReleasesEntityRetriever> releasesEntityRetrieverMock;

        private readonly CreateSaleInputModel testCreateSaleInputModel = new CreateSaleInputModel
        {
            VinylGrade = Condition.Mint,
            SleeveGrade = Condition.Fair,
            Price = 30,
            Description = "test description"
        };

        private readonly Address testAddress = new Address
        {
            Country = "Bulgaria",
            Town = "Sofia",
            PostalCode = "1612",
            FullAddress = "j.k Lagera blok 123"
        };

        public SalesServiceTests()
        {
            this.dbContext = DbFactory.CreateDbContext();

            this.addressesEntityRetrieverMock = new Mock<IAddressesEntityRetriever>();

            this.usersEntityRetrieverMock = new Mock<IUsersEntityRetriever>();

            this.releasesEntityRetrieverMock = new Mock<IReleasesEntityRetriever>();

            this.salesService = new SalesService(
                this.dbContext,
                this.addressesEntityRetrieverMock.Object,
                this.usersEntityRetrieverMock.Object,
                this.releasesEntityRetrieverMock.Object);
        }

        [Fact]
        public async Task CreateSaleShouldCreateSale()
        {
            var release = new Release();

            var seller = new VinylExchangeUser();

            var address = new Address();

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync(release);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(seller);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            this.testCreateSaleInputModel.ReleaseId = release.Id;

            this.testCreateSaleInputModel.ShipsFromAddressId = address.Id;

            var createdSaleModel =
                await this.salesService.CreateSale<CreateSaleResourceModel>(this.testCreateSaleInputModel, seller.Id);

            await this.dbContext.SaveChangesAsync();

            var createdSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == createdSaleModel.Id);

            Assert.NotNull(createdSale);
        }

        [Fact]
        public async Task CreateSaleShouldCreateSaleWithCorrectData()
        {
            var release = new Release();

            var seller = new VinylExchangeUser();

            var address = this.testAddress;

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync(release);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(seller);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            var addressProperties = new List<string> { address.Country, address.Town };

            this.testCreateSaleInputModel.ReleaseId = release.Id;

            this.testCreateSaleInputModel.ShipsFromAddressId = address.Id;

            var createdSaleModel =
                await this.salesService.CreateSale<CreateSaleResourceModel>(this.testCreateSaleInputModel, seller.Id);

            await this.dbContext.SaveChangesAsync();

            var createdSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == createdSaleModel.Id);

            Assert.Equal(this.testCreateSaleInputModel.VinylGrade, createdSale.VinylGrade);
            Assert.Equal(this.testCreateSaleInputModel.SleeveGrade, createdSale.SleeveGrade);
            Assert.Equal(this.testCreateSaleInputModel.ReleaseId, createdSale.ReleaseId);
            Assert.Equal(this.testCreateSaleInputModel.Price, createdSale.Price);
            Assert.True(addressProperties.Select(ap => createdSale.ShipsFrom.Contains(ap)).All(x => x));
        }

        [Fact]
        public async Task CreateSaleShouldThrowNullRefferenceExceptionIfProvidedReleaseIdIsNotInDb()
        {
            var address = new Address();

            var seller = new VinylExchangeUser();

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync((Release)null);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(seller);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.CreateSale<CreateSaleResourceModel>(
                                                this.testCreateSaleInputModel,
                                                seller.Id));

            Assert.Equal(ReleaseNotFound, exception.Message);
        }

        [Fact]
        public async Task CreateSaleShouldThrowNullRefferenceExceptionIfProvidedAddressIdIsNotInDb()
        {
            var release = new Release();

            var seller = new VinylExchangeUser();

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync(release);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(seller);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync((Address)null);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.CreateSale<CreateSaleResourceModel>(
                                                this.testCreateSaleInputModel,
                                                seller.Id));

            Assert.Equal(AddressNotFound, exception.Message);
        }

        [Fact]
        public async Task CreateSaleShouldThrowNullRefferenceExceptionIfProvidedUserIdIsNotInDb()
        {
            var release = new Release();

            var address = new Address();

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync(release);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>()))
                .ReturnsAsync((VinylExchangeUser)null);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.CreateSale<CreateSaleResourceModel>(
                                                this.testCreateSaleInputModel,
                                                Guid.NewGuid()));

            Assert.Equal(UserNotFound, exception.Message);
        }

        [Fact]
        public async Task EditSaleShouldEditSaleWithCorrectData()
        {
            var release = new Release();

            var user = new VinylExchangeUser();

            var updatedAddress = this.testAddress;

            this.releasesEntityRetrieverMock.Setup(x => x.GetRelease(It.IsAny<Guid?>())).ReturnsAsync(release);

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(user);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(updatedAddress);

            var addressProperties = new List<string> { updatedAddress.Country, updatedAddress.Town };

            var sale = new Sale
            {
                VinylGrade = Condition.Poor,
                SleeveGrade = Condition.NearMint,
                Description = "blbbebe",
                Price = 50,
                ShipsFrom = "Paris France"
            };

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var editSaleInputModel = new EditSaleInputModel
            {
                VinylGrade = Condition.Mint,
                SleeveGrade = Condition.Mint,
                Description = "updated description",
                Price = 100,
                SaleId = sale.Id
            };

            await this.salesService.EditSale<EditSaleResourceModel>(editSaleInputModel);

            var updatedSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

            Assert.Equal(editSaleInputModel.SaleId, updatedSale.Id);
            Assert.Equal(editSaleInputModel.VinylGrade, updatedSale.VinylGrade);
            Assert.Equal(editSaleInputModel.SleeveGrade, updatedSale.SleeveGrade);
            Assert.Equal(editSaleInputModel.Description, updatedSale.Description);
            Assert.Equal(editSaleInputModel.Price, updatedSale.Price);
            Assert.True(addressProperties.Select(ap => updatedSale.ShipsFrom.Contains(ap)).All(x => x));
        }

        [Fact]
        public async Task EditSaleShouldThrowNullRefferenceExceptionIfProvidedAddressIdIsNotInDb()
        {
            var sale = new Sale();

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync((Address)null);

            var editSaleInputModel = new EditSaleInputModel { SaleId = sale.Id, ShipsFromAddressId = Guid.NewGuid() };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.EditSale<EditSaleResourceModel>(
                                                editSaleInputModel));

            Assert.Equal(AddressNotFound, exception.Message);
        }

        [Fact]
        public async Task EditSaleShouldThrowNullRefferenceExceptionIfProvidedSaleIdIsNotInDb()
        {
            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(new Address());

            var editSaleInputModel = new EditSaleInputModel();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.EditSale<EditSaleResourceModel>(
                                                editSaleInputModel));

            Assert.Equal(SaleNotFound, exception.Message);
        }

        [Fact]
        public async Task RemoveSaleShouldRemoveSale()
        {
            var sale = new Sale();

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            await this.salesService.RemoveSale<RemoveSaleResourceModel>(sale.Id);

            var removedSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

            Assert.Null(removedSale);
        }

        [Fact]
        public async Task RemoveSaleShouldThrowNullReferenceExceptionIfProvidedGenreIdIsNotInDb()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                                async () => await this.salesService.RemoveSale<RemoveSaleResourceModel>(
                                                Guid.NewGuid()));

            Assert.Equal(SaleNotFound, exception.Message);
        }

        [Fact]
        public async Task GetAllSalesForReleaseShouldGetAllSalesForReleaseIfTheirStatusIsOpen()
        {
            var release = new Release();

            var secondRelease = new Release();

            var saleIds = new List<Guid?>();

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { Status = Status.Open, ReleaseId = release.Id };

                await this.dbContext.Sales.AddAsync(sale);

                saleIds.Add(sale.Id);
            }

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { Status = Status.Open, ReleaseId = secondRelease.Id };

                await this.dbContext.Sales.AddAsync(sale);

                saleIds.Add(sale.Id);
            }

            await this.dbContext.SaveChangesAsync();

            var saleModels =
                await this.salesService.GetAllSalesForRelease<GetAllSalesForReleaseResouceModel>(release.Id);

            Assert.True(saleModels.Count == release.Sales.Count);
            Assert.True(saleModels.Select(sm => saleIds.Contains(sm.Id)).All(x => x));
        }

        [Fact]
        public async Task GetAllSalesForReleaseShouldNotGetAnySAlesWithStatusOtherThanOpen()
        {
            var release = new Release();

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { Status = Status.Finished, ReleaseId = release.Id };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var saleModels =
                await this.salesService.GetAllSalesForRelease<GetAllSalesForReleaseResouceModel>(release.Id);

            Assert.True(saleModels.Count == 0);
        }

        [Fact]
        public async Task GetAllSalesForReleaseShouldNotGetAnySalesIfProvidedReleaseIdIsNotRegisteredByAnySale()
        {
            var release = new Release();

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { Status = Status.Finished, ReleaseId = release.Id };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var saleModels =
                await this.salesService.GetAllSalesForRelease<GetAllSalesForReleaseResouceModel>(Guid.NewGuid());

            Assert.True(saleModels.Count == 0);
        }

        [Fact]
        public async Task GetSaleShouldGetSaleIfProvidedSaleIsInDb()
        {
            var sale = new Sale();

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var saleModel = this.salesService.GetSale<GetSaleResourceModel>(sale.Id);

            Assert.NotNull(saleModel);
        }

        [Fact]
        public async Task GetSaleShouldReturnNullIfProvidedSaleIsNotInDb()
        {
            var sale = new Sale();

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var saleModel = await this.salesService.GetSale<GetSaleResourceModel>(Guid.NewGuid());

            Assert.Null(saleModel);
        }

        [Fact]
        public async Task GetUserPurchasesShouldGetUserPurchases()
        {
            var user = new VinylExchangeUser();

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { BuyerId = user.Id };

                await this.dbContext.Sales.AddAsync(sale);
            }

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { BuyerId = Guid.NewGuid() };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var purchasesModels = await this.salesService.GetUserPurchases<GetSaleResourceModel>(user.Id);

            Assert.True(purchasesModels.All(pm => pm.BuyerId == user.Id));
        }

        [Fact]
        public async Task GetUserPurchasesShouldReturnEmptyListIfUserHasNoPurchases()
        {
            var user = new VinylExchangeUser();

            for (var i = 0; i < 10; i++)
            {
                var sale = new Sale { BuyerId = Guid.NewGuid() };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var purchasesModels = await this.salesService.GetUserPurchases<GetSaleResourceModel>(user.Id);

            Assert.True(purchasesModels.Count == 0);
        }

        [Fact]
        public async Task GetUserSalesShouldGetUserSales()
        {
            var user = new VinylExchangeUser();

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { SellerId = user.Id };

                await this.dbContext.Sales.AddAsync(sale);
            }

            for (var i = 0; i < 5; i++)
            {
                var sale = new Sale { SellerId = Guid.NewGuid() };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var saleModels = await this.salesService.GetUserSales<GetSaleResourceModel>(user.Id);

            Assert.True(saleModels.All(sm => sm.SellerId == user.Id));
        }

        [Fact]
        public async Task GetUserSalesShouldReturnEmptyListIfUserHasNoSales()
        {
            var user = new VinylExchangeUser();

            for (var i = 0; i < 10; i++)
            {
                var sale = new Sale { SellerId = Guid.NewGuid() };

                await this.dbContext.Sales.AddAsync(sale);
            }

            await this.dbContext.SaveChangesAsync();

            var salesModels = await this.salesService.GetUserPurchases<GetSaleResourceModel>(user.Id);

            Assert.True(salesModels.Count == 0);
        }

        [Fact]
        public async Task PlaceOrderShouldChangeSaleStatusToShippingNegotiation()
        {
            var sale = new Sale { Status = Status.Open };

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var address = new Address();

            var user = new VinylExchangeUser();

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(user);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            await this.salesService.PlaceOrder<SaleStatusResourceModel>(sale.Id, address.Id, user.Id);

            var changedSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

            Assert.True(Status.ShippingNegotiation == changedSale.Status);
        }

        [Fact]
        public async Task PlaceOrderShouldSetBuyerId()
        {
            var sale = new Sale()
            {
                Status = Status.Open
            };

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var address = new Address();

            var user = new VinylExchangeUser();

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(user);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            await this.salesService.PlaceOrder<SaleStatusResourceModel>(sale.Id, address.Id, user.Id);

            var changedSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

            Assert.True(user.Id == changedSale.BuyerId);
        }

        [Fact]
        public async Task PlaceOrderShouldSetShipsToAddress()
        {
            var sale = new Sale()
            {
                Status = Status.Open
            };            

            await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            var address = new Address();

            var user = new VinylExchangeUser();

            this.usersEntityRetrieverMock.Setup(x => x.GetUser(It.IsAny<Guid?>())).ReturnsAsync(user);

            this.addressesEntityRetrieverMock.Setup(x => x.GetAddress(It.IsAny<Guid?>())).ReturnsAsync(address);

            await this.salesService.PlaceOrder<SaleStatusResourceModel>(sale.Id, address.Id, user.Id);

            var changedSale = await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);

            Assert.True(
                changedSale.ShipsTo
                == $"{address.Country} - {address.Town} - {address.PostalCode} - {address.FullAddress}");
        }
    }
}