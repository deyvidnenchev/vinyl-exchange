﻿using System;
using System.ComponentModel.DataAnnotations;
using VinylExchange.Data.Common.Enumerations;
using VinylExchange.Data.Models;
using VinylExchange.Services.Mapping;

namespace VinylExchange.Web.Models.InputModels.Sales
{
    public  class EditSaleInputModel : IMapTo<Sale>
    {
        [Required]
        public string Description { get; set; }

        [Required]

        public decimal Price { get; set; }

        [Required]
        public Guid? SaleId { get; set; }

        [Required]
        public Guid? ShipsFromAddressId { get; set; }

        [Required]
        public Condition SleeveGrade { get; set; }

        [Required]
        public Condition VinylGrade { get; set; }
    }
}