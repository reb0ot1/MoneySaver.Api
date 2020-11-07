using System.ComponentModel.DataAnnotations.Schema;

namespace MoneySaver.Api.Data
{
    public class TransactionCategory
    {
        [Column("Id")]
        public int TransactionCategoryId { get; set; }
        public string Name { get; set; }
    } 
}