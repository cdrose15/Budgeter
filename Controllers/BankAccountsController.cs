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
            //var HHid = !string.IsNullOrWhiteSpace(HHidS) ?Convert.ToInt32(HHidS): 0;
            var accounts = db.Accounts.Where(b => b.HouseholdId == HHid);

            ViewBag.Overdraft = "Overdraft Warning";
            
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
        public ActionResult Create([Bind(Include = "Id,Name,Balance,HouseholdId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var user = Convert.ToInt32(User.Identity.GetHouseholdId());
                bankAccount.HouseholdId = user;
                db.Accounts.Add(bankAccount);
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
            if (bankAccount  == null)
            {
                return HttpNotFound();
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
            db.Accounts.Remove(bankAccount);
            db.SaveChanges();
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
