﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VinylExchange.Models.ResourceModels.Genres;

namespace VinylExchange.Services.MainServices
{
    public interface IGenresService
    {
        Task<IEnumerable<GetAllGenresResourceModel>> GetAllGenres();
    }
}
