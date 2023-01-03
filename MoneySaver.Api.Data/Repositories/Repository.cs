using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IUser, IDeletable, new()
    {
        protected MoneySaverApiContext databaseContext;
        private readonly UserPackage userPackage;
        private readonly ILogger<Repository<TEntity>> logger;

        public Repository(
            MoneySaverApiContext context, 
            UserPackage userPackage,
            ILogger<Repository<TEntity>> logger)
        {
            this.databaseContext = context;
            this.userPackage = userPackage;
            this.logger = logger;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.UserId = this.userPackage.UserId;
            await this.databaseContext.AddAsync(entity);
            await this.databaseContext.SaveChangesAsync();

            return entity;
        }

        public IQueryable<TEntity> GetAll()
        { 
            var collection = this.databaseContext
                .Set<TEntity>()
                .Where(w => w.UserId == this.userPackage.UserId && !w.IsDeleted);

            return collection;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity.UserId != this.userPackage.UserId)
            {
                this.logger
                    .LogError($"Forbbiden operation from user id {this.userPackage.UserId} to an entity which is property to user id {entity.UserId}");
                throw new System.Exception($"Forbbiden operation from user id {this.userPackage.UserId} to an entity which is property to user id {entity.UserId}");
            }

            this.databaseContext.Update(entity);
            await this.databaseContext.SaveChangesAsync();

            return entity;
        }

        public async Task SetAsDeletedAsync(TEntity entity)
        {
            if (entity.UserId != this.userPackage.UserId)
            {
                this.logger
                    .LogError($"Forbbiden operation from user id {this.userPackage.UserId} to an entity which is property to user id {entity.UserId}");
                return;
            }

            this.databaseContext.Update(entity);

            await this.databaseContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            if (entity.UserId != this.userPackage.UserId)
            {
                this.logger
                    .LogError($"Forbbiden operation from user id {this.userPackage.UserId} to an entity which is property to user id {entity.UserId}");
                return;
            }

            this.databaseContext.Remove(entity);

            await this.databaseContext.SaveChangesAsync();
        }
    }
}
