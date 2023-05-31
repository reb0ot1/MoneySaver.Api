namespace MoneySaver.Api.Models.Request
{
    public class UpdateBudgetRequest : CreateBudgetRequest
    {
        public bool IsInUse { get; set; }
    }
}
