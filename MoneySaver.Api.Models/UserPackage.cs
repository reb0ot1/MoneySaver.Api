using System;

namespace MoneySaver.Api.Models
{
    public class UserPackage
    {
        public string UserId { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
