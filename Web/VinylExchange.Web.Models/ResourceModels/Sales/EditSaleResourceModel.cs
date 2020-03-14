﻿using System;
using VinylExchange.Data.Models;
using VinylExchange.Services.Mapping;

namespace VinylExchange.Web.Models.ResourceModels.Sales
{
    public class EditSaleResourceModel : IMapFrom<Sale>
    {
        public Guid Id { get; set; }
    }
}
