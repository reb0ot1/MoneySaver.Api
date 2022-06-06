using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Models.Request
{
    public class PageRequest
    {
        public int ItemsToSkip { get; set; }

        public int ItemsPerPage { get; set; }

        //TODO: Include filter. The below is only for test
        public string Filter { get; set; }
    }
}
