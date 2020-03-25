﻿namespace VinylExchange.Web.Controllers
{
    #region

    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using VinylExchange.Services.Logging;
    using VinylExchange.Services.MemoryCache;
    using VinylExchange.Web.Models.InputModels.Files;
    using VinylExchange.Web.Models.ResourceModels.File;
    using VinylExchange.Web.Models.ResourceModels.Files;
    using VinylExchange.Web.Models.Utility;

    #endregion

    public class FilesController : ApiController
    {
        private readonly ILoggerService loggerService;

        private readonly IMemoryCacheFileSevice memoryCacheFileSevice;

        public FilesController(IMemoryCacheFileSevice memoryCacheFileSevice, ILoggerService loggerService)
        {
            this.memoryCacheFileSevice = memoryCacheFileSevice;
            this.loggerService = loggerService;
        }

        [HttpDelete]
        [Route("DeleteAll")]
        public ActionResult<IEnumerable<RemoveAllFilesForSessionResourceModel>> RemoveAllFilesForSession([FromQuery]RemoveAllFilesForSessionInputModel inputModel)
        {
            try
            {
                return this.memoryCacheFileSevice.RemoveAllFilesForSession<RemoveAllFilesForSessionResourceModel>(inputModel);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);

                return this.BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult<DeleteFileResourceModel> DeleteFile(Guid id, Guid formSessionId)
        {
            try
            {
                return this.memoryCacheFileSevice.RemoveFile(formSessionId, id);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);

                return this.BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<UploadFileResourceModel> UploadFile(IFormFile file, Guid formSessionId)
        {
            try
            {
                var fileModel = new UploadFileUtilityModel(file);

                return this.memoryCacheFileSevice.AddFile(fileModel, formSessionId);
            }
            catch (Exception ex)
            {
                this.loggerService.LogException(ex);

                return this.BadRequest();
            }
        }
    }
}