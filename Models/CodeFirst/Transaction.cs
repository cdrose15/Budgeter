using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        public int BankAccountId { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Type { get; set; }
        public bool Reconciled { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual Category Category { get; set; }
    } 
}