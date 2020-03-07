﻿namespace VinylExchange.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;

    using VinylExchange.Data.Common.Enumerations;
    using VinylExchange.Data.Models;
    using VinylExchange.Services.Data.MainServices.Sales;
    using VinylExchange.Services.Logging;
    using VinylExchange.Web.Hubs.SaleLog;
    using VinylExchange.Web.Models.InputModels.Sales;
    using VinylExchange.Web.Models.ResourceModels.Sales;
    using VinylExchange.Web.Models.Utility;

    [Authorize]
    public class SalesController : ApiController
    {
        private readonly ILoggerService loggerService;

        private readonly IHubContext<SaleLogsHub, ISaleLogsClient> saleLogHubContext;

        private readonly ISalesService salesService;

        public SalesController(
            ISalesService salesService,
            ILoggerService loggerService,
            IHubContext<SaleLogsHub, ISaleLogsClient> saleLogHubContext)
        {
            this.salesService = salesService;
            this.loggerService = loggerService;
            this.saleLogHubContext = saleLogHubContext;
        }

        [HttpPut]
        [Route("CompletePayment")]
        public async Task<IActionResult> CompletePayment(CompletePaymentInputModel inputModel)
        {
            try
            {
                GetSaleInfoUtilityModel saleModel = await this.salesService.GetSaleInfo(inputModel.SaleId);

                if (saleModel == null)
                {
                    return this.BadRequest();
                }

                Guid currentUserId = this.GetUserId(this.User);

                if (saleModel.BuyerId == currentUserId && saleModel.Status == Status.PaymentPending)
                {
                    await this.salesService.CompletePayment(inputModel);

                    return this.NoContent();
                }
                else
                {
                    return this.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSaleInputModel inputModel)
        {
            try
            {
                Sale sale = await this.salesService.CreateSale(inputModel, this.GetUserId(this.User));

                return this.StatusCode(HttpStatusCode.Created, sale.Id);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                GetSaleResourceModel sale = await this.salesService.GetSale(id);

                if (sale == null)
                {
                    return this.NotFound();
                }

                Guid currentUserId = this.GetUserId(this.User);

                if ((sale.BuyerId != currentUserId && sale.SellerId != currentUserId) && sale.Status != Status.Open)
                {
                    return this.Unauthorized();
                }

                return this.Ok(sale);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpGet]
        [Route("GetAllSalesForRelease/{id}")]
        public async Task<IActionResult> GetAllSalesForRelease(Guid id)
        {
            try
            {
                IEnumerable<GetAllSalesForReleaseResouceModel> sales = await this.salesService.GetAllSalesForRelease(id);

                return this.Ok(sales);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpGet]
        [Route("GetUserPurchases")]
        public async Task<IActionResult> GetUserPurchases()
        {
            try
            {
                IEnumerable<GetUserPurchasesResourceModel> purchases =
                    await this.salesService.GetUserPurchases(this.GetUserId(this.User));

                return this.Ok(purchases);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpGet]
        [Route("GetUserSales")]
        public async Task<IActionResult> GetUserSales()
        {
            try
            {
                IEnumerable<GetUserSalesResourceModel> sales =
                    await this.salesService.GetUserSales(this.GetUserId(this.User));

                return this.Ok(sales);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpPut]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(PlaceOrderInputModel inputModel)
        {
            try
            {
                GetSaleInfoUtilityModel saleModel = await this.salesService.GetSaleInfo(inputModel.SaleId);

                if (saleModel == null)
                {
                    return this.BadRequest();
                }

                Guid currentUserId = this.GetUserId(this.User);

                if (saleModel.SellerId == currentUserId || saleModel.BuyerId == currentUserId
                                                        || saleModel.Status != Status.Open)
                {
                    return this.Unauthorized();
                }

                await this.salesService.PlaceOrder(inputModel, currentUserId);

                await this.saleLogHubContext.Clients.Group(saleModel.Id.ToString()).RecieveNewLog("test");

                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }

        [HttpPut]
        [Route("SetShippingPrice")]
        public async Task<IActionResult> SetShippingPrice(SetShippingPriceInputModel inputModel)
        {
            try
            {
                GetSaleInfoUtilityModel saleModel = await this.salesService.GetSaleInfo(inputModel.SaleId);

                if (saleModel == null)
                {
                    return this.BadRequest();
                }

                Guid currentUserId = this.GetUserId(this.User);

                if (saleModel.SellerId == currentUserId && saleModel.Status == Status.ShippingNegotiation)
                {
                    await this.salesService.SetShippingPrice(inputModel);

                    return this.NoContent();
                }
                else
                {
                    return this.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);
                return this.BadRequest();
            }
        }
    }
}