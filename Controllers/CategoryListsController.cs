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
    [Authorize]
    public class CategoryListsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CategoryLists
        public ActionResult Index()
        {
            return View(db.CategoryLists.ToList());
        }

        // GET: CategoryLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryList categoryList = db.CategoryLists.Find(id);
            if (categoryList == null)
            {
                return HttpNotFound();
            }
            return View(categoryList);
        }

        // GET: CategoryLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CategoryList categoryList)
        {
            if (ModelState.IsValid)
            {
                db.CategoryLists.Add(categoryList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoryList);
        }

        // GET: CategoryLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryList categoryList = db.CategoryLists.Find(id);
            if (categoryList == null)
            {
                return HttpNotFound();
            }
            return View(categoryList);
        }

        // POST: CategoryLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] CategoryList categoryList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoryList);
        }

        // GET: CategoryLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryList categoryList = db.CategoryLists.Find(id);
            if (categoryList == null)
            {
                return HttpNotFound();
            }
            return View(categoryList);
        }

        // POST: CategoryLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoryList categoryList = db.CategoryLists.Find(id);
            db.CategoryLists.Remove(categoryList);
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
