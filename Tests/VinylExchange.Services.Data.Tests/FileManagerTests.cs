﻿namespace VinylExchange.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Files;
    using MemoryCache.Contracts;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Web.Models.Utility.Files;
    using Xunit;

    public class FileManagerTests
    {
        public FileManagerTests()
        {
            this.memoryCacheFilesServiceMock = new Mock<IMemoryCacheFilesSevice>();

            this.fileManager = new FileManager(this.memoryCacheFilesServiceMock.Object);
        }

        private readonly FileManager fileManager;

        private readonly Mock<IMemoryCacheFilesSevice> memoryCacheFilesServiceMock;

        [Fact]
        public void GetFilesByteContentShouldGetByteContentFromCollectionOfUploadFileUtilityModel()
        {
            var random = new Random();

            var filesForUploadModels = new List<UploadFileUtilityModel>();

            for (var i = 0; i < 10; i++)
            {
                var uploadFileUtilityModel =
                    new UploadFileUtilityModel(new FormFile(new MemoryStream(), 100L, 100L, "test", "test.jpg"));

                var byteArr = new byte[30];

                random.NextBytes(byteArr);

                uploadFileUtilityModel.FileByteContent = byteArr;

                filesForUploadModels.Add(uploadFileUtilityModel);
            }

            var byteContent = this.fileManager.GetFilesByteContent(filesForUploadModels);

            Assert.Equal(string.Join(",", filesForUploadModels.Select(x => x.FileByteContent)),
                string.Join(",", byteContent));
        }
    }
}