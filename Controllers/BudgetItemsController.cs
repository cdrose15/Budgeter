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
using System.Web.Helpers;

namespace Budgeter.Controllers
{
    [RoutePrefix("Budget")]
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var budgetItems = db.BudgetItems.OrderBy(b => b.Type).Where(b => b.CategoryId == b.Category.Id
                && b.HouseholdId == user);
            ViewBag.SuccessMessage = TempData["successMessage"];
            return View(budgetItems.ToList());
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        public PartialViewResult _Create()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var budgetCategoryList = db.Categories.Where(b => b.HouseholdId == user && b.IsDeleted != true);
            ViewBag.CategoryId = new SelectList(budgetCategoryList, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(db.Households.Where(h => h.Id == user), "Id", "Name");
            return PartialView();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,CategoryId,Name,Amount,HouseholdId")] BudgetItem budgetItem)
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (ModelState.IsValid)
            {
                budgetItem.HouseholdId = user;
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetItem.HouseholdId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Edit/5
        public PartialViewResult _Edit(int? id)
        {

            BudgetItem budgetItem = db.BudgetItems.Find(id);

            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var budgetCategoryList = db.Categories.Where(b => b.HouseholdId == user);
            var budgetHousehold = db.BudgetItems.Where(b => b.HouseholdId == user);

            ViewBag.CategoryId = new SelectList(budgetCategoryList, "Id", "Name", budgetItem.CategoryId);
            ViewBag.HouseholdId = new SelectList(budgetHousehold, "Id", "Name", budgetItem.HouseholdId);
            return PartialView(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,CategoryId,Name,Amount,HouseholdId")] BudgetItem budgetItem)
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (ModelState.IsValid)
            {
                budgetItem.HouseholdId = user;
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetItem.HouseholdId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);

            return PartialView(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItems.Find(id);
            db.BudgetItems.Remove(budgetItem);
            db.SaveChanges();
            TempData["successMessage"] = "Budget Item successfully deleted";
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

