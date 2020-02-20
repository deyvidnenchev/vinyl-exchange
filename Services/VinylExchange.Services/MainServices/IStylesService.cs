﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VinylExchange.Models.ResourceModels.Styles;

namespace VinylExchange.Services.MainServices
{
    public interface IStylesService
    {
        Task<IEnumerable<GetAllStylesResourceModel>> GetAllStylesForGenre(int genreId);
    }
}
