namespace MoneySaver.Api.Models
{
    public class TransactionCategoryModel
    {
        public int? TransactionCategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
