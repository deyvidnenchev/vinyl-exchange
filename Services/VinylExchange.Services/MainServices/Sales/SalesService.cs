﻿namespace VinylExchange.Services.Data.MainServices.Sales
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using VinylExchange.Common.Constants;
    using VinylExchange.Data;
    using VinylExchange.Data.Common.Enumerations;
    using VinylExchange.Data.Models;
    using VinylExchange.Services.Mapping;
    using VinylExchange.Web.Models.InputModels.Sales;

    #endregion

    public class SalesService : ISalesService
    {
        private readonly VinylExchangeDbContext dbContext;

        public SalesService(VinylExchangeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TModel> CompletePayment<TModel>(CompletePaymentInputModel inputModel)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            sale.Status = Status.Paid;
            sale.OrderId = inputModel.OrderId;

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        public async Task<TModel> ConfirmItemRecieved<TModel>(ConfirmItemRecievedInputModel inputModel)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            sale.Status = Status.Finished;

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        public async Task<TModel> ConfirmItemSent<TModel>(ConfirmItemSentInputModel inputModel)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            sale.Status = Status.Sent;

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        public async Task<TModel> CreateSale<TModel>(CreateSaleInputModel inputModel, Guid sellerId)
        {
            var sale = inputModel.To<Sale>();

            var address = await this.GetAddress(inputModel.ShipsFromAddressId);

            if (address == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.AddressNotFound);
            }

            sale.ShipsFrom = $"{address.Country} - {address.Town}";

            sale.SellerId = sellerId;

            sale.Status = Status.Open;

            var trackedSale = await this.dbContext.Sales.AddAsync(sale);

            await this.dbContext.SaveChangesAsync();

            return trackedSale.Entity.To<TModel>();
        }

        public async Task<TModel> EditSale<TModel>(EditSaleInputModel inputModel)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            if (sale == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.SaleNotFound);
            }

            var address = await this.dbContext.Addresses.Where(a => a.Id == inputModel.ShipsFromAddressId)
                              .FirstOrDefaultAsync();

            if (address == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.AddressNotFound);
            }

            sale.Price = inputModel.Price;

            sale.SleeveGrade = inputModel.SleeveGrade;

            sale.VinylGrade = inputModel.VinylGrade;

            sale.Description = inputModel.Description;

            sale.ShipsFrom = $"{address.Country} - {address.Town}";

            sale.Status = Status.Open;

            sale.ModifiedOn = DateTime.UtcNow;

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        public async Task<List<TModel>> GetAllSalesForRelease<TModel>(Guid releaseId)
        {
            return await this.dbContext.Sales.Where(s => s.ReleaseId == releaseId).Where(s => s.Status == Status.Open)
                       .To<TModel>().ToListAsync();
        }

        public async Task<TModel> GetSale<TModel>(Guid saleId)
        {
            return await this.dbContext.Sales.Where(s => s.Id == saleId).To<TModel>().FirstOrDefaultAsync();
        }

        public async Task<TModel> GetSaleInfo<TModel>(Guid? saleId)
        {
            return await this.dbContext.Sales.Where(s => s.Id == saleId).To<TModel>().FirstOrDefaultAsync();
        }

        public async Task<List<TModel>> GetUserPurchases<TModel>(Guid buyerId)
        {
            return await this.dbContext.Sales.Where(s => s.BuyerId == buyerId).To<TModel>().ToListAsync();
        }

        public async Task<List<TModel>> GetUserSales<TModel>(Guid sellerId)
        {
            return await this.dbContext.Sales.Where(s => s.SellerId == sellerId).To<TModel>().ToListAsync();
        }

        public async Task<TModel> PlaceOrder<TModel>(PlaceOrderInputModel inputModel, Guid? buyerId)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            var address = await this.GetAddress(inputModel.AddressId);

            if (address == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.AddressNotFound);
            }

            sale.BuyerId = buyerId;
            sale.Status = Status.ShippingNegotiation;
            sale.ShipsTo = $"{address.Country} - {address.Town} - {address.PostalCode} - {address.FullAddress}";

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        public async Task<TModel> RemoveSale<TModel>(Guid saleId)
        {
            var sale = await this.GetSale(saleId);

            if (sale == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.SaleNotFound);
            }

            var removedAddress = this.dbContext.Sales.Remove(sale).Entity;
            await this.dbContext.SaveChangesAsync();

            return removedAddress.To<TModel>();
        }

        public async Task<TModel> SetShippingPrice<TModel>(SetShippingPriceInputModel inputModel)
        {
            var sale = await this.GetSale(inputModel.SaleId);

            sale.ShippingPrice = inputModel.ShippingPrice;
            sale.Status = Status.PaymentPending;

            await this.dbContext.SaveChangesAsync();

            return sale.To<TModel>();
        }

        private async Task<Address> GetAddress(Guid? addressId)
        {
            return await this.dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
        }

        private async Task<Sale> GetSale(Guid? saleId)
        {
            return await this.dbContext.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
        }
    }
}