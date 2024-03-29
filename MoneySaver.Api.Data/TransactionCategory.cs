﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneySaver.Api.Data
{
    public class TransactionCategory : IUser, IDeletable
    {
        [Column("Id")]
        public int TransactionCategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public string UserId { get; set; }
    } 
}