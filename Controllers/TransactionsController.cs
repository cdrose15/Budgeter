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

        // GET: Transactions
        public ActionResult Index()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var transactions = db.Transactions.Where(t => t.BankAccountId  == t.BankAccount.Id && t.BankAccount.HouseholdId == user).Include(t => t.Category);
            return View(transactions.ToList());
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {

            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accountList = db.Accounts.Where(a => a.HouseholdId == user);

            ViewBag.BankAccountId = new SelectList(accountList.ToList(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,BankAccountId,CategoryId,Description,Type,Reconciled,Amount")] Transaction transaction)
        {
            var id = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);

            if (ModelState.IsValid)
            {
                if (transaction.Type == true)
                {
                    account.Balance = (account.Balance - transaction.Amount);
                    db.SaveChanges();
                }

                if (transaction.Type == false)
                {
                    account.Balance = (account.Balance + transaction.Amount);
                    db.SaveChanges();
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.Accounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountId = new SelectList(db.Accounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,BankAccountId,CategoryId,Description,Type,Reconciled,Amount")] Transaction transaction)
        {

            if (ModelState.IsValid)
            {
                var originalBalance = db.Transactions.AsNoTracking().FirstOrDefault(o => o.Id == transaction.Id);
                var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);

                // Reverse original balance calculation
                if (transaction.Type == true)
                {
                    account.Balance = (account.Balance + originalBalance.Amount);
                } 
                if(transaction.Type == false)
                {
                    account.Balance = (account.Balance - originalBalance.Amount);
                }

                // New edit balance calculation
                if (transaction.Type == true)
                {
                    account.Balance = (account.Balance - transaction.Amount);
                    db.SaveChanges();
                }
                if (transaction.Type == false)
                {
                    account.Balance = (account.Balance + transaction.Amount);
                    db.SaveChanges();
                }

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.Accounts, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
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
                db.SaveChanges();
            }
            if(transaction.Type == false)
            {
                account.Balance = (account.Balance - transaction.Amount);
                db.SaveChanges();
            }
           
            return RedirectToAction("Index");
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
