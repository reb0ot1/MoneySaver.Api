namespace MoneySaver.Api.Mcp.Models
{
    public class CategoryModel
    {
        public int TransactionCategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string AlternativeName { get; set; }
        public List<CategoryModel> Children { get; set; } = new List<CategoryModel>();
    }
}
