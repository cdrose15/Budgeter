using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models.CodeFirst
{
    public class Invite
    {
        public int Id { get; set; }
        public string NameId { get; set; }
        public int HouseholdId { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
        [Required]
        public string InviteCode { get; set; }

        public virtual Household Household { get; set; }
        public virtual ApplicationUser Name { get; set; }

    }
}