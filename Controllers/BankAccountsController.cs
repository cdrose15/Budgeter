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
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accounts = db.Accounts.Where(b => b.HouseholdId == user && b.IsDeleted != true);

            if (user == 52)
            {
                ViewBag.DemoMessage = "Glad to see your still using this Demo! On this page you can " +
                    "create, edit, and delete accounts. Each account has a link to access the transactions " +
                    "associated with it as well. You also can transfer amounts from and to specified accounts.";
            }
            ViewBag.OverdraftWarning = "Overdraft Warning";
            ViewBag.Overdraft = "Overdraft Notice";
            ViewBag.ErrorMessage = TempData["errorMessage"];
            ViewBag.SuccessMessage = TempData["successMessage"];
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

        // GET: BankAccounts/Transfer
        public PartialViewResult _Transfer(BankAccount bankAccount)
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var fromList = db.Accounts.Where(f => f.HouseholdId == user && f.IsDeleted != true);
            var toList = db.Accounts.Where(t => t.HouseholdId == user && t.IsDeleted != true);

            ViewBag.fromId = new SelectList(fromList.ToList(), "Id", "Name",bankAccount.Id);
            ViewBag.toId = new SelectList(toList.ToList(), "Id", "Name", bankAccount.Id);

            return PartialView();
        }

        //POST: BankAccounts/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]
        public ActionResult Transfer(int fromId, int toId, decimal transferamount)
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());

            var fromAccount = db.Accounts.Find(fromId);
            var toAccount = db.Accounts.Find(toId);

            Transaction fromTransaction = new Transaction();
            fromTransaction.Amount = transferamount;
            fromTransaction.Description = "Transfer";
            fromTransaction.Date = DateTime.Now;
            fromTransaction.Type = true;
            fromTransaction.BankAccountId = fromAccount.Id;
            fromTransaction.Reconciled = true;
            db.Transactions.Add(fromTransaction);
            fromAccount.Balance = fromAccount.Balance - transferamount;
            fromAccount.ReconciledBalance = fromAccount.Balance - transferamount;
            db.SaveChanges();

            Transaction toTransaction = new Transaction();
            toTransaction.Amount = transferamount;
            toTransaction.Description = "Transfer";
            toTransaction.Date = DateTime.Now;
            toTransaction.Type = false;
            toTransaction.BankAccountId = toAccount.Id;
            toTransaction.Reconciled = true;
            db.Transactions.Add(toTransaction);
            toAccount.Balance = toAccount.Balance + transferamount;
            toAccount.ReconciledBalance = toAccount.Balance + transferamount;
            db.SaveChanges();

            TempData["successMessage"] = "Transfer completed";
            return RedirectToAction("Index");
        }


        // GET: BankAcounts/Details/5
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
            if(user == 52)
            {
                ViewBag.DemoMessage = "Go ahead and create, edit, and delete a transaction from this page. Do everyone " +
                    "a favor and make sure you keep some around for the other Demo users to see though.";
            }
            if (bankAccount  == null)
            {
                return HttpNotFound();
            }

            if(bankAccount.IsDeleted == true)
            {
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
                TempData["errorMessage"] = "Account balance must be $0 in order to delete, please transfer your remaining balance to another account";
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
            var accounts = db.Accounts.Where(b => b.HouseholdId == HHid && b.IsDeleted != true);

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
