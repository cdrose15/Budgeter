using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class BudgetItem
    {
        public int Id { get; set; }
        [Required]
        public bool Type { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        public int Frequency { get; set; }
        public int HouseholdId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Household Household { get; set; }
    }
}