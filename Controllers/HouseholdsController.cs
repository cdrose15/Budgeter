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
using System.Threading.Tasks;

namespace Budgeter.Controllers
{
    [RequireHttps]
   
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        //GET: Households/Index
        [AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);
            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            ViewBag.SuccessMessage = TempData["successMessage"];
            return View(household);
        }

        [HttpGet]
        //GET: Households/Join

        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        //POST: Households/Join
        public async Task<ActionResult> Joins(string Code)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user == null)
                {
                    var currUserEmail = User.Identity.GetUserName();
                    var invite = db.Invites.FirstOrDefault(c => c.InviteCode == Code && c.UserEmail == currUserEmail);
                    if (invite != null)
                    {
                        user.HouseholdId = invite.HouseholdId;
                        db.Invites.Remove(invite);
                        // Refreshauthentication() call
                        var userid = User.Identity.GetUserId();
                        await ControllerContext.HttpContext.RefreshAuthentication(user);
                        db.SaveChanges();
                        return RedirectToAction("Index", new { id = user });
                    }
                    else
                    {
                        TempData["errorMessage"] = "An error has occurred";
                        return RedirectToAction("Create", "Households");
                    }
                }
                else
                {
                    return RedirectToAction("Index", new { id = user });
                }            
            }
            TempData["errorMessage"] = "An error has occurred";
            return RedirectToAction("Create","Households");
        }

        [HttpPost]
        // POST: Households/Leave
        //[ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]

        public async Task<ActionResult> Leave()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            user.HouseholdId = null;
            // Refreshauthentication() call
            var userid = User.Identity.GetUserId();
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            db.SaveChanges();

            return RedirectToAction("Create","Households");           
        }

        [HttpGet]
        // GET: Households/Create
        public ActionResult Create()
        {
            ViewBag.ErrorMessage = TempData["errorMessage"];
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.HouseholdId == null)
                {
                    db.Households.Add(household);
                    db.SaveChanges();

                    var hh = db.Households.FirstOrDefault(h => h.Name == household.Name);
                    user.HouseholdId = hh.Id;
                    // Refreshauthentication() call
                    var userid = User.Identity.GetUserId();
                    await ControllerContext.HttpContext.RefreshAuthentication(user);
                    db.SaveChanges();

                    // Populate CategoryList to new Households Category
                    var Cats = db.CategoryLists.ToList();
                    var catsList = new List<Category>();
                    foreach (var cat in Cats)
                    {
                        var category = new Category()
                        {
                            Name = cat.Name,
                            HouseholdId = household.Id
                        };
                        catsList.Add(category);
                    }
                    db.Categories.AddRange(catsList);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { id = hh.Id });
                }
                else
                {
                    return RedirectToAction("Index", new { id = user.HouseholdId });
                }
            }

            ViewBag.errorMessage = "An error has occurred";
            return View();
        }

        // POST: Households/Invite
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeHouseholdRequired]

        public ActionResult Invite(ContactMessage sendemail)
        {

            var invite = new Invite();
            var keycode = KeyGenerator.GetUniqueKey(5);

            invite.NameId = User.Identity.GetUserId();
            var user = db.Users.Find(invite.NameId);
            invite.UserEmail = sendemail.Email;
            invite.HouseholdId = sendemail.HouseholdId;
            invite.InviteCode = keycode;

            var duplicates = db.Invites.Where(i => i.UserEmail == sendemail.Email);
            foreach(var duplicate in duplicates)
            {
                db.Invites.Remove(duplicate);
            }

            db.Invites.Add(invite);
            db.SaveChanges();

            var Email = new EmailService();

            var mail = new IdentityMessage
            {
                Subject = "Join My Household",
                Destination = sendemail.Email,
                Body = $"{user.FirstName } { user.LastName } sent you a message. Enter the following code: {invite.InviteCode} to join my household. First you must go to https://crose-budget.azurewebsites.net and register."
            };

            Email.SendAsync(mail);

            TempData["successMessage"] = "Your Message Has Been Successfully Sent";
            return RedirectToAction("Index", "Households");
        }

        // GET: Households/Dashboard
        [HttpGet]
        [AuthorizeHouseholdRequired]

        public ActionResult Dashboard()
        {
            return View();
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
