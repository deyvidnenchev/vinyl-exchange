﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VinylExchange.Models.Utility;
using VinylExchange.Tools;

namespace VinylExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly MemoryCacheManager cache;

        public FileController(MemoryCacheManager cache)
        {
            this.cache = cache;
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult DeleteFile(string formSessionId, string fileGuid)
        {


            var key = cache.GetKeys().Where(x => x == formSessionId).SingleOrDefault();

            var formSessionStorage = cache.Get<List<UploadImageUtility>>(key, null);

            var image = formSessionStorage.SingleOrDefault(x => x.FileGuid.ToString() == fileGuid);

            if (image != null) {
                formSessionStorage.Remove(image);

                var returnObj = new
                {
                    removed = image.File.FileName,
                    filesStillInStorage = string.Join(",", formSessionStorage.Select(x => x.File.FileName))
                };

                return Json(returnObj);
            }
            else
            {
                return BadRequest("there is no file with this fileGuid in cache storage");
            }

        }

        [HttpPost]
        [Route("upload")]
        public IActionResult UploadFile(IFormFile file, string formSessionId)
        {
            var imageGuid = Guid.NewGuid();

            UploadImageUtility image = new UploadImageUtility(file, imageGuid);

            if (!cache.IsSet(formSessionId))
            {
                cache.Set(formSessionId, new List<UploadImageUtility>(), 1000);
            }

            var formSessionStorage = cache.Get<List<UploadImageUtility>>(formSessionId, null);

            formSessionStorage.Add(image);

            var returnObj = new
            {
                added = image.File.FileName,
                guid = imageGuid,
                filesInStorage = string.Join(",", formSessionStorage.Select(x => x.File.FileName))
            };

            return Json(returnObj);
        }


    }
}
