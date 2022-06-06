using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Models.Reports
{
    public class SeriesItem
    {
        public string Name { get; set; }

        public double?[] Data { get; set; }
    }
}
