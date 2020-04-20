﻿namespace VinylExchange.Services.Data.HelperServices.Users
{
    #region

    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using VinylExchange.Common.Constants;
    using VinylExchange.Data;
    using VinylExchange.Data.Models;
    using VinylExchange.Models.InputModels.Users;
    using VinylExchange.Services.Mapping;
    using VinylExchange.Web.Models.ResourceModels.UsersAvatar;

    #endregion

    public class UsersAvatarService : IUsersAvatarService
    {
        private readonly VinylExchangeDbContext dbContext;

        public UsersAvatarService(VinylExchangeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<VinylExchangeUser> ChangeAvatar(byte[] avatar, Guid? userId)
        {
            var user = await this.dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NullReferenceException(NullReferenceExceptionsConstants.UserNotFound);
            }
            
            user.Avatar = avatar;

            await this.dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<GetUserAvatarResourceModel> GetUserAvatar(Guid? userId)
        {
            return await this.dbContext.Users.Where(u => u.Id == userId).To<GetUserAvatarResourceModel>()
                       .FirstOrDefaultAsync();
        }
    }
}