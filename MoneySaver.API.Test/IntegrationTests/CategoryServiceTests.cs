using MoneySaver.Api.Models;
using MoneySaver.API.Test.SeedData;

namespace MoneySaver.API.Test.IntegrationTests
{
    public class CategoryServiceTests : IClassFixture<CategoryContext>
    {
        private readonly CategoryContext _context;

        public CategoryServiceTests(CategoryContext context)
        {
            this._context = context;
            this._context.ClearDataAsync().GetAwaiter().GetResult();
            this._context.SeedDataAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public async void GetAllGategories_WhenNoFilters()
        {
            //Arrange
            var categoryService = this._context.GetService();
            var allCategories = await categoryService.GetAllCategoriesAsync();
            var initialCount = allCategories.Data.Count();
            var newCategory =  new TransactionCategoryModel
            {
                Name = "Test category2",
            };
            
            //Act
            await categoryService.CreateCategoryAsync(newCategory);
            var allCategoriesAfterUpdate = await categoryService.GetAllCategoriesAsync();
            
            //Assert
            Assert.NotNull(allCategoriesAfterUpdate);
            Assert.NotNull(allCategoriesAfterUpdate.Succeeded);
            Assert.Equal(initialCount + 1, allCategoriesAfterUpdate.Data.Count());
        }

        [Fact]
        public async void CheckValidName_WhenCallingAllCategories()
        {
            //Arrange
            var parentCategoryName = "TestCat1";
            var categoryService = this._context.GetService();
            var allCategories = await categoryService.GetAllCategoriesAsync();
            var initialCount = allCategories.Data.Count();
            var catToCheck = allCategories.Data?.FirstOrDefault(e => e.Name == parentCategoryName);
            var newCategory =  new TransactionCategoryModel
            {
                Name = "Test Child Category",
                ParentId = catToCheck?.TransactionCategoryId
            };
            
            //Act
            var createdCategory = await categoryService.CreateCategoryAsync(newCategory);
            var allCategoriesUpdated = await categoryService.GetAllCategoriesAsync();
            var searchedCategory = allCategoriesUpdated.Data.FirstOrDefault(e => e.TransactionCategoryId == createdCategory.Data.TransactionCategoryId);
            
            //Assert
            Assert.NotNull(searchedCategory);
            Assert.Equal(newCategory.Name, searchedCategory.Name);
            Assert.Equal(catToCheck.TransactionCategoryId, searchedCategory.ParentId);
            Assert.Equal(initialCount + 1, allCategoriesUpdated.Data.Count());
        }

        [Fact]
        public async void GetCategory_ById()
        {
            //Arrange
            var categoryService = this._context.GetService();
            var allCategories = categoryService.GetAllCategoriesAsync();
            var randomCategory = allCategories.Result.Data.FirstOrDefault();
            
            //Act
            var searchedCategory = await categoryService.GetCategoryAsync(randomCategory.TransactionCategoryId.Value);

            //Assert
            Assert.NotNull(searchedCategory);
            Assert.NotNull(searchedCategory.Succeeded);
            Assert.Equal(randomCategory.TransactionCategoryId.Value, searchedCategory.Data.TransactionCategoryId);
            Assert.Equal(randomCategory.Name, searchedCategory.Data.Name);
        }

        [Fact]
        public async void UpdateCategory_ById()
        {
            //Arrange
            var categoryService = this._context.GetService();
            var allCategories = await categoryService.GetAllCategoriesAsync();
            var randomCategory = allCategories.Data.FirstOrDefault();
            var modelForUpdate = new TransactionCategoryModel
            {
                Name = "Updated name",
                TransactionCategoryId = randomCategory.TransactionCategoryId.Value,
                ParentId = randomCategory.ParentId
            };

            //Act
            var searchedCategory = await categoryService.UpdateCategoryAsync(modelForUpdate);

            //Assert
            Assert.NotNull(searchedCategory);
            Assert.NotNull(searchedCategory.Succeeded);
            Assert.Equal(modelForUpdate.TransactionCategoryId, searchedCategory.Data.TransactionCategoryId);
            Assert.Equal(modelForUpdate.Name, searchedCategory.Data.Name);
            Assert.Equal(modelForUpdate.ParentId, searchedCategory.Data.ParentId);
        }
    }
}
