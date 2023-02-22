namespace MoneySaver.Api.Models.Shared
{
    public class IdValue<T>
    {
        public int Id { get; set; }

        public T Value { get; set; }
    }
}
