using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, new()
    {

        protected MoneySaverApiContext databaseContext;

        public Repository(MoneySaverApiContext context)
        {
            this.databaseContext = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await this.databaseContext.AddAsync(entity);
            await this.databaseContext.SaveChangesAsync();

            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return this.databaseContext.Set<TEntity>();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            this.databaseContext.Update(entity);
            await this.databaseContext.SaveChangesAsync();

            return entity;
        }

        public async void RemoveAsync(TEntity entity)
        {
            //this.databaseContext.Remove(entity);
            this.databaseContext.Update(entity);

            await this.databaseContext.SaveChangesAsync();
        }
    }
}
