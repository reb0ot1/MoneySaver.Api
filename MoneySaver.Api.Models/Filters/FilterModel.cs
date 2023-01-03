using System;

namespace MoneySaver.Api.Models.Filters
{
    public class FilterModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string SearchText { get; set; }

        public int[] CategoryIds { get; set; } = new int[] { };
    }
}
