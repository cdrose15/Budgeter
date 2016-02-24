using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Household
    {
        public Household()
        {
            this.BankAccounts = new HashSet<BankAccount>();
            this.Categories = new HashSet<Category>();
            this.BudgetItems = new HashSet<BudgetItem>();
            this.Invites = new HashSet<Invite>();
            this.Users = new HashSet<ApplicationUser >();        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection <BankAccount> BankAccounts { get; set; }
        public virtual ICollection <Category> Categories { get; set; }
        public virtual ICollection <BudgetItem> BudgetItems { get; set; }
        public virtual ICollection <Invite> Invites { get; set; }
        public virtual ICollection <ApplicationUser> Users { get; set; }
    }
}