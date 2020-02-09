﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VinylExchange.Models.Utility;
using VinylExchange.Services.MemoryCache;
using System.IO;

namespace VinylExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IMemoryCacheFileSevice memoryCacheFileSevice;

        public FileController(IMemoryCacheFileSevice memoryCacheFileSevice)
        {
            this.memoryCacheFileSevice = memoryCacheFileSevice;
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult DeleteFile(string formSessionId, string fileGuid)
        {

            var returnObj = this.memoryCacheFileSevice.RemoveFile(formSessionId, fileGuid);

            return Ok(returnObj);

        }

        [HttpPost]
        [Route("DeleteAll")]
        public IActionResult DeleteAllFiles(string formSessionId)
        {

            this.memoryCacheFileSevice.RemoveAllFilesForFormSession(formSessionId);

            return Ok();


        }


        [HttpPost]
        [Route("Upload")]
        public IActionResult UploadFile(IFormFile file, string formSessionId)
        {
           
            UploadFileUtilityModel fileModel = new UploadFileUtilityModel(file);

            var returnObj = this.memoryCacheFileSevice.AddFile(fileModel, formSessionId);

            return Ok(returnObj);
        }


    }
}
