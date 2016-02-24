using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Budgeter.Models.CodeFirst;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: BankAccounts
        [AuthorizeHouseholdRequired]
        public ActionResult Index(BankAccount bankAccount)
        {
            var HHidS = User.Identity.GetHouseholdId();
            int HHid;
            if (!string.IsNullOrWhiteSpace(HHidS))
            {
                HHid = Convert.ToInt32(HHidS);
            }
            else
                HHid = 0;
            var accounts = db.Accounts.Where(b => b.HouseholdId == HHid && b.IsDeleted != true);

            ViewBag.OverdraftWarning = "Overdraft Warning";
            ViewBag.Overdraft = "Overdraft Notice";
            ViewBag.ErrorMessage = TempData["errorMessage"];
            return View(accounts.ToList());            
        }

        // GET: BankAccounts/Create
        public PartialViewResult _Create()
        {
            return PartialView();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]
        public ActionResult Create([Bind(Include = "Id,Name,Balance,HouseholdId,ReconciledBalance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var user = Convert.ToInt32(User.Identity.GetHouseholdId());
                bankAccount.HouseholdId = user;
                bankAccount.ReconciledBalance = bankAccount.Balance;
                db.Accounts.Add(bankAccount);
                db.SaveChanges();

                Transaction transaction = new Transaction();
                transaction.Amount = bankAccount.Balance;
                transaction.Description = "Initial Deposit";
                transaction.Date = DateTime.Now;
                transaction.Type = false;
                transaction.BankAccountId = bankAccount.Id;
                transaction.Reconciled = true;
                db.Transactions.Add(transaction);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.Accounts.Find(id);
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var transactions = db.Transactions.OrderBy(t => t.Date).Where(t => t.BankAccountId  == t.BankAccount.Id &&
                t.BankAccount.HouseholdId == user).Include(t => t.Category);
            if (bankAccount  == null)
            {
                return HttpNotFound();
            }

            if(bankAccount.IsDeleted == true)
            {
                ViewBag.IsDeletedMessage = "This account has been deleted. You can only view transactions.";
                return View(bankAccount);
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            BankAccount bankAccount = db.Accounts.Find(id);
       
            return PartialView(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,HouseholdId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            BankAccount bankAccount = db.Accounts.Find(id);

            return PartialView(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.Accounts.Find(id);
            if (bankAccount.Balance == 0)
            {
                bankAccount.IsDeleted = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = "Account balance must be $0 in order to delete, please manually move your remaining balance to another account";
                return RedirectToAction("Index");
            }
         
        }

        // GET: BankAccounts/_AccountList
        public PartialViewResult _AccountList(BankAccount bankAccount)
        {
            var HHidS = User.Identity.GetHouseholdId();
            int HHid;
            if (!string.IsNullOrWhiteSpace(HHidS))
            {
                HHid = Convert.ToInt32(HHidS);
            }
            else
                HHid = 0;
            var accounts = db.Accounts.Where(b => b.HouseholdId == HHid);

            ViewBag.OverdraftWarning = "Overdraft Warning";
            ViewBag.Overdraft = "Overdraft Notice";

            return PartialView(accounts.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
