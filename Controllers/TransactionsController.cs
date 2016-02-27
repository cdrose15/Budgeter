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
    [AuthorizeHouseholdRequired]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: _TransactionResult
        public PartialViewResult _TransactionList()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var transactions = db.Transactions.OrderByDescending(t => t.Date).Where(t => t.BankAccountId == t.BankAccount.Id &&
                t.BankAccount.HouseholdId == user);
            return PartialView(transactions.ToList().Take(4));
        }

        // GET: Transactions/Create

        public PartialViewResult _Create()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accountList = db.Accounts.Where(a => a.HouseholdId == user && a.IsDeleted != true);
            var categoryList = db.Categories.Where(c => c.HouseholdId == user && c.IsDeleted != true);

            ViewBag.BankAccountId = new SelectList(accountList.ToList(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(categoryList.ToList(), "Id", "Name");
            return PartialView();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,BankAccountId,CategoryId,Description,Type,Reconciled,Amount")] Transaction transaction)
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);

            if (ModelState.IsValid)
            {
                if (transaction.Type == true)
                {
                    account.Balance = (account.Balance - transaction.Amount);

                    if(transaction.Reconciled == true)
                    {
                        account.ReconciledBalance = (account.ReconciledBalance - transaction.Amount);
                    }

                    db.SaveChanges();
                }

                if (transaction.Type == false)
                {
                    account.Balance = (account.Balance + transaction.Amount);

                    if (transaction.Reconciled == true)
                    {
                        account.ReconciledBalance = (account.ReconciledBalance + transaction.Amount);
                    }

                    db.SaveChanges();
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();
                
                return RedirectToAction("Details","BankAccounts", new { id = transaction.BankAccountId });
            }

            ViewBag.BankAccountId = new SelectList(db.Accounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Transaction transaction = db.Transactions.Find(id);

            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var categoryList = db.Categories.Where(c => c.HouseholdId == user && c.IsDeleted != true);
            var accountList = db.Accounts.Where(a => a.HouseholdId == user && a.IsDeleted != true);

            ViewBag.BankAccountId = new SelectList(accountList, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(categoryList, "Id", "Name", transaction.CategoryId);
            return PartialView(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,BankAccountId,CategoryId,Description,Type,Reconciled,Amount")] Transaction transaction)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var userBankAccount = db.Accounts.Where(u => u.HouseholdId == user && u.IsDeleted != true);
            var userCategoryList = db.Categories.Where(u => u.HouseholdId == user && u.IsDeleted != true);

            if (ModelState.IsValid)
            {
                var originalBal = db.Transactions.AsNoTracking().FirstOrDefault(o => o.Id == transaction.Id);
                var originalReconBal = db.Transactions.AsNoTracking().FirstOrDefault(o => o.Id == transaction.Id);

                account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);
     
                // Reverse original balance calculation
                if (originalBal.Type  == true)
                {
                    account.Balance = (account.Balance + originalBal.Amount);

                    if (originalReconBal.Reconciled  == true)
                    {
                        account.ReconciledBalance = (account.ReconciledBalance + originalReconBal.Amount);
                    }

                    db.SaveChanges();
                }
                if (originalBal.Type == false)
                {
                    account.Balance = (account.Balance - originalBal.Amount);

                    if (originalReconBal.Reconciled == true)
                    {
                        account.ReconciledBalance = (account.ReconciledBalance - originalReconBal.Amount);
                    }

                    db.SaveChanges();
                }

                // New edit balance calculation
                if (transaction.Type == true)
                {
                    account.Balance = (account.Balance - transaction.Amount);

                    if (transaction.Reconciled == true)
                    {
                        account.ReconciledBalance = account.ReconciledBalance - transaction.Amount;
                    }

                    db.SaveChanges();
                }
                if (transaction.Type == false)
                {
                    account.Balance = (account.Balance + transaction.Amount);

                    if (transaction.Reconciled == true)
                    {
                        account.ReconciledBalance = account.ReconciledBalance + transaction.Amount;
                    }

                    db.SaveChanges();
                }

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
            }
            ViewBag.BankAccountId = new SelectList(userBankAccount, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(userCategoryList, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            Transaction transaction = db.Transactions.Find(id);
     
            return PartialView(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();

            var hh= User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == hh);
            var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);

            if(transaction.Type == true)
            {
                account.Balance = (account.Balance + transaction.Amount);

                if (transaction.Reconciled == true)
                {
                    account.ReconciledBalance = account.ReconciledBalance + transaction.Amount;
                }

                db.SaveChanges();
            }
            if(transaction.Type == false)
            {
                account.Balance = (account.Balance - transaction.Amount);

                if (transaction.Reconciled == true)
                {
                    account.ReconciledBalance = account.ReconciledBalance - transaction.Amount;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
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
