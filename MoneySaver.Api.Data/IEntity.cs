using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Api.Data
{
    public interface IEntity
    {
        string Id { get; set; }

        DateTime CreatedOn { get; set; }

        DateTime ModifiedOn { get; set; }
    }
}
