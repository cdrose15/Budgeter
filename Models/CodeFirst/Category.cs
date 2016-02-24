using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Category
    {
        public Category()
        {
            this.Transactions = new HashSet<Transaction>();
            this.BudgetItems = new HashSet<BudgetItem>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? HouseholdId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction > Transactions { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
    }
}