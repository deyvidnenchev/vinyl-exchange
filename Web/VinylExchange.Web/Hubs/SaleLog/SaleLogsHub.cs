﻿namespace VinylExchange.Web.Hubs.SaleLog
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using VinylExchange.Services.Data.HelperServices.Sales.SaleLogs;
    using VinylExchange.Services.Data.HelperServices.Sales.SaleMessages;
    using VinylExchange.Services.Data.MainServices.Sales;
    using VinylExchange.Web.Models.Utility;

    [Authorize]
    public class SaleLogsHub : Hub<ISaleLogsClient>
    {
       
        private readonly ISalesService salesService;
        private readonly ISaleLogsService saleLogsService;

        public SaleLogsHub(ISalesService salesService, ISaleLogsService saleLogsService)
        {
            this.salesService = salesService;
            this.saleLogsService = saleLogsService;
        }

        public async Task SubscribeToLog(Guid saleId)
        {
            var subscriberGroupName = saleId.ToString();

            GetSaleInfoUtilityModel sale = await this.salesService.GetSaleInfo(saleId);

            Guid userId = Guid.Parse(this.GetUserId());

            if (sale != null)
            {
                if (sale.SellerId == userId || sale.BuyerId == userId)
                {
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, subscriberGroupName);
                }
            }
        }

        public async Task LoadLogHistory(Guid saleId)
        {
            GetSaleInfoUtilityModel sale = await this.salesService.GetSaleInfo(saleId);

            Guid userId = Guid.Parse(this.GetUserId());

            if (sale != null)
            {
                if (sale.SellerId == userId || sale.BuyerId == userId)
                {
                    var logs = await this.saleLogsService.GetLogsForSale(saleId);

                    await this.Clients.Caller.LoadLogHistory(logs);
                }
            }
        }

        private string GetUserId()
        {
            return this.Context.User.FindFirst("sub").Value;
        }
    }
}