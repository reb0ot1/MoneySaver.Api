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
        }

        [Fact]
        public async void GetAllGategories_WhenNoFilters()
        {
            //Arrange
            var categoryService = this._context.GetService();

            //Act
            var allCategories = await categoryService.GetAllCategoriesAsync();

            //Assert
            Assert.NotNull(allCategories);
            Assert.NotNull(allCategories.Succeeded);
            Assert.Equal(allCategories.Data.Count(), 2);
        }

        [Fact]
        public async void CheckValidName_WhenCallingAllCategories()
        {
            //Arrange
            var catIdToCheck = 3;
            var categoryService = this._context.GetService();

            //Act
            var allCategories = await categoryService.GetAllCategoriesAsync();
            var catToCheck = allCategories.Data?.FirstOrDefault(e => e.TransactionCategoryId == catIdToCheck);

            //Assert
            Assert.NotNull(catToCheck);
            Assert.Equal(catToCheck.TransactionCategoryId, catIdToCheck);
            Assert.Equal(catToCheck.ParentId, 1);
            Assert.Equal(catToCheck.Name, "TestCat3");
        }

        [Fact]
        public async void GetCategory_ById()
        {
            //Arrange
            var catIdToCheck = 3;
            var categoryService = this._context.GetService();

            //Act
            var searchedCategory = await categoryService.GetCategoryAsync(catIdToCheck);

            //Assert
            Assert.NotNull(searchedCategory);
            Assert.NotNull(searchedCategory.Succeeded);
            Assert.Equal(searchedCategory.Data.TransactionCategoryId, catIdToCheck);
            Assert.Equal(searchedCategory.Data.Name, "TestCat3");
        }

        [Fact]
        public async void UpdateCategory_ById()
        {
            //Arrange
            var catIdToCheck = 3;
            var parrentId = 4;
            var categoryService = this._context.GetService();
            var modelForUpdate = new TransactionCategoryModel
            {
                Name = "Updated name",
                TransactionCategoryId = catIdToCheck,
                ParentId = parrentId
            };

            //Act
            var searchedCategory = await categoryService.UpdateCategoryAsync(modelForUpdate);

            //Assert
            Assert.NotNull(searchedCategory);
            Assert.NotNull(searchedCategory.Succeeded);
            Assert.Equal(searchedCategory.Data.TransactionCategoryId, catIdToCheck);
            Assert.Equal(searchedCategory.Data.Name, modelForUpdate.Name);
            Assert.Equal(searchedCategory.Data.ParentId, modelForUpdate.ParentId);
        }
    }
}
