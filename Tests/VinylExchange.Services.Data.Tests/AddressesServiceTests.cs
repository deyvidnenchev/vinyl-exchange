﻿namespace VinylExchange.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Constants;
    using MainServices.Addresses;
    using Microsoft.EntityFrameworkCore;
    using TestFactories;
    using VinylExchange.Data;
    using VinylExchange.Data.Models;
    using Web.Models.ResourceModels.Addresses;
    using Web.Models.Utility.Addresses;
    using Xunit;

    public class AddressesServiceTests
    {
        public AddressesServiceTests()
        {
            this.dbContext = DbFactory.CreateDbContext();

            this.addressesService = new AddressesService(this.dbContext);
        }

        private readonly AddressesService addressesService;

        private readonly VinylExchangeDbContext dbContext;

        [Fact]
        public async Task CreateAddressShouldCreateAddress()
        {
            var createdAddressModel = await this.addressesService.CreateAddress<CreateAddressResourceModel>(
                "Bulgaria",
                "Sofia",
                "1612",
                "Test",
                Guid.NewGuid());

            await this.dbContext.SaveChangesAsync();

            var createdAddress =
                await this.dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == createdAddressModel.Id);

            Assert.NotNull(createdAddress);
        }

        [Fact]
        public async Task CreateAddressShouldCreateAddressWithCorrectData()
        {
            var createdAddressModel = await this.addressesService.CreateAddress<CreateAddressResourceModel>(
                "Bulgaria",
                "Sofia",
                "1612",
                "Test",
                Guid.NewGuid());

            await this.dbContext.SaveChangesAsync();

            var createdAddress =
                await this.dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == createdAddressModel.Id);

            Assert.Equal("Bulgaria", createdAddress.Country);
            Assert.Equal("Sofia", createdAddress.Town);
            Assert.Equal("1612", createdAddress.PostalCode);
            Assert.Equal("Test", createdAddress.FullAddress);
        }

        [Fact]
        public async Task GetAddressShouldGetAddress()
        {
            var address = new Address();

            await this.dbContext.Addresses.AddAsync(address);

            await this.dbContext.SaveChangesAsync();

            var returnedAddress = await this.addressesService.GetAddress(address.Id);

            Assert.NotNull(returnedAddress);
        }

        [Fact]
        public async Task GetAddressShouldReturnCorrectAddress()
        {
            var address = (await this.dbContext.Addresses.AddAsync(new Address())).Entity;

            await this.dbContext.SaveChangesAsync();

            var returnedAddressModel = await this.addressesService.GetAddress<GetAddressInfoUtilityModel>(address.Id);

            Assert.Equal(address.Id, returnedAddressModel.Id);
        }

        [Fact]
        public async Task GetAddressShouldReturnNullIfProvidedAddressIdIsNotExistingInDb()
        {
            await this.dbContext.Addresses.AddAsync(new Address());

            await this.dbContext.SaveChangesAsync();

            var returnedAddressModel =
                await this.addressesService.GetAddress<GetAddressInfoUtilityModel>(Guid.NewGuid());

            Assert.Null(returnedAddressModel);
        }

        [Fact]
        public async Task GetUserAddressesShouldReturnEmptyListWhenANonExistingUserIdIsProvided()
        {
            var userId = Guid.NewGuid();

            for (var i = 0; i < 3; i++)
            {
                await this.dbContext.Addresses.AddAsync(new Address {UserId = userId});
            }

            await this.dbContext.SaveChangesAsync();

            var userAddressesModels =
                await this.addressesService.GetUserAddresses<GetAddressInfoUtilityModel>(Guid.NewGuid());

            Assert.True(userAddressesModels.Count == 0);
        }

        [Fact]
        public async Task GetUserAddressesShouldReturnUserAddresses()
        {
            var userId = Guid.NewGuid();

            for (var i = 0; i < 3; i++)
            {
                await this.dbContext.Addresses.AddAsync(new Address {UserId = userId});
            }

            await this.dbContext.SaveChangesAsync();

            var userAddressesModels = await this.addressesService.GetUserAddresses<GetAddressInfoUtilityModel>(userId);

            Assert.True(userAddressesModels.All(a => a.UserId == userId));
        }

        [Fact]
        public async Task RemoveAddressShouldRemoveAddress()
        {
            var address = (await this.dbContext.Addresses.AddAsync(new Address())).Entity;

            await this.dbContext.SaveChangesAsync();

            await this.addressesService.RemoveAddress<RemoveAddressResourceModel>(address.Id);

            var removedAddress = await this.dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == address.Id);

            Assert.Null(removedAddress);
        }

        [Fact]
        public async Task RemoveAddressShouldThrowNullReferenceExceptionIfProvidedAddressIdDoesntBelongToAnyAddress()
        {
            await this.dbContext.Addresses.AddAsync(new Address());

            await this.dbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                async () =>
                    await this.addressesService.RemoveAddress<RemoveAddressResourceModel>(
                        Guid.NewGuid()));

            Assert.Equal(NullReferenceExceptionsConstants.AddressNotFound, exception.Message);
        }
    }
}