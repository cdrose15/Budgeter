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

namespace Budgeter.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var user = Convert.ToInt32(User.Identity.GetHouseholdId());
            var categories = db.Categories.Where(c => c.HouseholdId == user && c.IsDeleted != true).OrderBy(c => c.Name);

            ViewBag.SuccessMessage = TempData["successMessage"];

            return View(categories.ToList());
        }

        // GET: Categories/Create
        public PartialViewResult _Create()
        {
            return PartialView();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseholdId")] Category category)
        {
            if (ModelState.IsValid)
            {
                var user = Convert.ToInt32(User.Identity.GetHouseholdId());
                category.HouseholdId = user;               
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Category category = db.Categories.Find(id);

            return PartialView(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseholdId")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            Category category = db.Categories.Find(id);

            return PartialView(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //    Category category = db.Categories.Find(id);
            //    if (category.Name == "Misc.")
            //    {
            //        TempData["errorMessage"] = "Sorry, you cannot delete this category";
            //        return RedirectToAction("Index");
            //    }
            //    else
            //    {
            //        db.Categories.Remove(category);
            //        db.SaveChanges();
            //        TempData["successMessage"] = "Category successfully deleted";
            //        return RedirectToAction("Index");
            //    }
            //}

            Category category = db.Categories.Find(id);
            category.IsDeleted = true;
            db.SaveChanges();
            TempData["successMessage"] = "Category successfully deleted";
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
