using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.API.Test;
using MoneySaver.API.Test.Database;

public class TransactionContext : IAsyncLifetime
{
    private TransactionService _transactionService;
    private TransactionCategoryService _transactionCategoryService;
    private MoneySaverApiContext dbContext;
    private UserPackage userPackage = new UserPackage { UserId = Guid.NewGuid().ToString(), IsAdmin = true };
    private DatabaseInMemorySetup _dbInMemory;
    private IDateProvider _dateTimeProvider = new DateProvider();
    
    public async Task InitializeAsync()
    {
        var dbSetup = new DatabaseInMemorySetup();
        this._dbInMemory = new DatabaseInMemorySetup();
        this.dbContext = this._dbInMemory.GetDatabase();
        await this.dbContext.Database.EnsureCreatedAsync();


        var mapper = MockingProviders.GenerateMapperInstance();
        var transactionCategoryRepository =
            MockingProviders.GetMockedRepository<TransactionCategory>(this.dbContext, this.userPackage);
        
        
        this._transactionService = new TransactionService(
            MockingProviders.GetMockedRepository<Transaction>(this.dbContext, this.userPackage),
            transactionCategoryRepository,
            mapper,
            MockingProviders.GetMockedLogger<TransactionService>().Object,
            this.userPackage
        );
        
        this._transactionCategoryService = new TransactionCategoryService(
            MockingProviders.GetMockedLogger<TransactionCategoryService>().Object,
            transactionCategoryRepository,
            mapper,
            userPackage
        );
    }

    public async Task ClearDataAsync()
    {
        await this.dbContext.Set<Transaction>().Where(e => true).ExecuteDeleteAsync();
        await this.dbContext.Set<BudgetItem>().Where(e => true).ExecuteDeleteAsync();
        await this.dbContext.Set<Budget>().Where(e => true).ExecuteDeleteAsync();
        await this.dbContext.Set<TransactionCategory>().Where(e => true).ExecuteDeleteAsync();
    }

    public async Task SeedDataAsync()
    {
        var cat1 = await this.dbContext.AddAsync<TransactionCategory>(
            new TransactionCategory
            {
                Name = "TestCat1",
                UserId = this.userPackage.UserId
            });

        var cat2 = await this.dbContext.AddAsync<TransactionCategory>(
            new TransactionCategory
            {
                Name = "TestCat2",
                UserId = this.userPackage.UserId
            });
        
        // await this.dbContext.AddAsync<TransactionCategory>(
        //     new TransactionCategory
        //     {
        //         Name = "TestCat3",
        //         UserId = this.userPackage.UserId,
        //     });
        //
        // await this.dbContext.AddAsync<TransactionCategory>(
        //     new TransactionCategory
        //     {
        //         Name = "TestCat4",
        //         UserId = this.userPackage.UserId
        //     });

        await this.dbContext.SaveChangesAsync();
        
        await this.dbContext.Set<Transaction>()
            .AddAsync(new Transaction
            {
                Amount = 10,
                TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                UserId = this.userPackage.UserId,
                TransactionDate = _dateTimeProvider.GetDateTimeNow().AddMonths(-1),
            });

        await this.dbContext.Set<Transaction>()
            .AddAsync(new Transaction
            {
                Amount = 30,
                TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                UserId = this.userPackage.UserId,
                TransactionDate = _dateTimeProvider.GetDateTimeNow().AddMonths(-1),
            });
            
        await this.dbContext.SaveChangesAsync();
    }

    public TransactionService TransactionService => _transactionService;
    public TransactionCategoryService TransactionCategoryService => _transactionCategoryService;
    
    public IDateProvider DateProvider => _dateTimeProvider;
    
    public async Task DisposeAsync()
    {
        await this._dbInMemory.RemoveAsync();
    }
}