using MoneySaver.API.Test.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoneySaver.API.Test
{
    [CollectionDefinition(nameof(BudgetCollection))]
    public class BudgetCollection : IClassFixture<BudgetContext>
    {
    }
}
