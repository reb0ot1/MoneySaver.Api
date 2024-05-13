using AutoMapper;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using Moq;

namespace MoneySaver.API.Test
{
    public class MockingProviders
    {
        public static Mock<ILogger<TEntity>> GetMockedLogger<TEntity>()
        {
            return new Mock<ILogger<TEntity>>();
        }

        public static IRepository<TEntity> GetMockedRepository<TEntity>(MoneySaverApiContext dbContext, UserPackage user) where TEntity : class, IUser, IDeletable, new()
        {
            var logger = GetMockedLogger<Repository<TEntity>>();
            return new Repository<TEntity>(dbContext, user, logger.Object);
        }

        public static Mapper GenerateMapperInstance()
        {
            var myProfile = new Api.Mapper();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            
            return new Mapper(configuration);
        }
    }
}
