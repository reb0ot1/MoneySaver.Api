using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Models.Reports
{
    public class LineChartData
    {
        public string[] Categories { get; set; }

        public List<SeriesItem> Series { get; set; }
    }
}
