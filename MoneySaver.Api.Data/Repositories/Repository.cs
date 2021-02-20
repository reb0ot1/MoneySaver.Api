using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IUser, new()
    {
        protected MoneySaverApiContext databaseContext;
        private readonly UserPackage userPackage;

        public Repository(
            MoneySaverApiContext context, 
            UserPackage userPackage)
        {
            this.databaseContext = context;
            this.userPackage = userPackage;
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
            var collection = this.databaseContext.Set<TEntity>();

            return collection;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UserId = this.userPackage.UserId;
            this.databaseContext.Update(entity);
            await this.databaseContext.SaveChangesAsync();

            return entity;
        }

        public async Task RemoveAsync(TEntity entity)
        {
            //TODO: Should have entities which can be deleted from the database and such which will  but with IsDeleted flag set true
            //this.databaseContext.Remove(entity);
            if (entity.UserId != this.userPackage.UserId)
            {
                return;
            }

            this.databaseContext.Update(entity);

            await this.databaseContext.SaveChangesAsync();
        }
    }
}
