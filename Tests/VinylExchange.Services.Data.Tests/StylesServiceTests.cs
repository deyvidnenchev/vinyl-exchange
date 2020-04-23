﻿namespace VinylExchange.Services.Data.Tests
{
    using System;
    using System.Threading.Tasks;
    using MainServices.Genres.Contracts;
    using MainServices.Styles;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using TestFactories;
    using VinylExchange.Data;
    using VinylExchange.Data.Models;
    using Web.Models.ResourceModels.Styles;
    using Xunit;
    using static Common.Constants.NullReferenceExceptionsConstants;


    public class StylesServiceTests
    {
        private readonly VinylExchangeDbContext dbContext;

        private readonly IStylesService stylesService;

        private readonly Mock<IGenresEntityRetriever> genresEntityRetrieverMock;

        public StylesServiceTests()
        {
            dbContext = DbFactory.CreateDbContext();

            genresEntityRetrieverMock = new Mock<IGenresEntityRetriever>();

            stylesService = new StylesService(dbContext, genresEntityRetrieverMock.Object);
        }

        [Fact]
        public async Task CreateStyleShouldCreateStyle()
        {
            var name = "Electronic";

            var genre = new Genre();

            genresEntityRetrieverMock.Setup(x => x.GetGenre(It.IsAny<int?>())).ReturnsAsync(genre);

            var createdStyleModel = await stylesService.CreateStyle<CreateStyleResourceModel>(name, genre.Id);

            await dbContext.SaveChangesAsync();

            var createdStyle = await dbContext.Styles.FirstOrDefaultAsync(s => s.Id == createdStyleModel.Id);

            Assert.NotNull(createdStyle);
        }

        [Fact]
        public async Task CreateStyleShouldCreateStyleWithCorrectData()
        {
            var name = "Electronic";

            var genre = new Genre();

            genresEntityRetrieverMock.Setup(x => x.GetGenre(It.IsAny<int?>())).ReturnsAsync(genre);

            var createdStyleModel = await stylesService.CreateStyle<CreateStyleResourceModel>(name, genre.Id);

            await dbContext.SaveChangesAsync();

            var createdStyle = await dbContext.Styles.FirstOrDefaultAsync(s => s.Id == createdStyleModel.Id);

            Assert.Equal(name, createdStyle.Name);
            Assert.Equal(genre.Id, createdStyle.GenreId);
        }

        [Fact]
        public async Task CreateStyleShouldThrowNullReferenceExceptionIfProvidedGenreIdIsNotInDb()
        {
            genresEntityRetrieverMock.Setup(x => x.GetGenre(It.IsAny<int>())).ReturnsAsync((Genre) null);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                async () => await stylesService.CreateStyle<CreateStyleResourceModel>("test", 2));

            Assert.Equal(GenreNotFound, exception.Message);
        }

        [Fact]
        public async Task GetAllStylesForGenreShouldGetAllStylesForProvidedValidGenreId()
        {
            var genreId = 38421;
            var secondGenreId = 23123;

            for (var i = 0; i < 5; i++)
            {
                dbContext.Styles.Add(new Style {Name = "Electronic", GenreId = genreId});
            }

            dbContext.Styles.Add(new Style {Name = "Metal", GenreId = secondGenreId});

            await dbContext.SaveChangesAsync();

            var styleModels = await stylesService.GetAllStylesForGenre<GetAllStylesForGenreResourceModel>(genreId);

            Assert.True(styleModels.Count == 5);
        }

        [Fact]
        public async Task GetAllStylesForGenreShouldReturnAllStylesWhenGenreIdIsNotProvided()
        {
            var genreId = 38421;
            var secondGenreId = 23123;

            for (var i = 0; i < 5; i++)
            {
                dbContext.Styles.Add(new Style {Name = "Electronic", GenreId = genreId});
            }

            dbContext.Styles.Add(new Style {Name = "Metal", GenreId = secondGenreId});

            await dbContext.SaveChangesAsync();

            var styleModels = await stylesService.GetAllStylesForGenre<GetAllStylesForGenreResourceModel>(null);

            Assert.True(styleModels.Count == 6);
        }

        [Fact]
        public async Task GetAllStylesForGenreShouldReturnEmptyListWhenANonExistingGenreIdIsProvided()
        {
            var genreId = 38421;
            var secondGenreId = 23123;

            for (var i = 0; i < 5; i++)
            {
                dbContext.Styles.Add(new Style {Name = "Electronic", GenreId = genreId});
            }

            await dbContext.SaveChangesAsync();

            var styleModels =
                await stylesService.GetAllStylesForGenre<GetAllStylesForGenreResourceModel>(secondGenreId);

            Assert.True(styleModels.Count == 0);
        }

        [Fact]
        public async Task RemoveStyleShouldRemoveStyle()
        {
            var style = (await dbContext.Styles.AddAsync(new Style {Id = 5})).Entity;

            await dbContext.SaveChangesAsync();

            await stylesService.RemoveStyle<RemoveStyleResourceModel>(style.Id);

            var removeStyle = await dbContext.Genres.FirstOrDefaultAsync(s => s.Id == style.Id);

            Assert.Null(removeStyle);
        }

        [Fact]
        public async Task RemoveStyleShouldThrowNullReferenceExceptionIfProvidedStyleIdIsNotInDb()
        {
            var style = (await dbContext.Styles.AddAsync(new Style {Id = 5})).Entity;

            await dbContext.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                async () => await stylesService.RemoveStyle<RemoveStyleResourceModel>(23));

            Assert.Equal(StyleNotFound, exception.Message);
        }
    }
}