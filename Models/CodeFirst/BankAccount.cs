using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class BankAccount
    {
        public BankAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public int HouseholdId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection <Transaction> Transactions { get; set; }
    }
}