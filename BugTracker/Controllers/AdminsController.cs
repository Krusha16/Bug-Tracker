using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllRoles()
        {
            var roles = MembershipHelper.GetAllRoles();
            return View(roles);
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRole(string Name)
        {
            MembershipHelper.AddRole(Name);
            return RedirectToAction("AllRoles");
        }

        [HttpGet]
        public ActionResult AssignUserToRole()
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AssignUserToRole(string UserId, string role)
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            var newrole = db.Roles.Where(r => r.Id == role).Select(r => r.Name);
            MembershipHelper.AddUserToRole(UserId, newrole.FirstOrDefault());
            return RedirectToAction("AllRoles");
        }

        [HttpGet]
        public ActionResult UnAssignUserFromRole()
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult UnAssignUserFromRole(string UserId, string role)
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            if(MembershipHelper.RemoveUserFromRole(UserId, role))
            {
                ViewBag.Message = String.Format("User has been removed from the role!");
            }
            else
            {
                ViewBag.Message = String.Format("Something went wrong!");
            }
            return RedirectToAction("AllRoles");
        }

        public ActionResult GetAllRolesForUser()
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            return View();
        }

        [HttpPost]
        public ActionResult GetAllRolesForUser(string UserId)
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            var roles = MembershipHelper.GetAllRolesOfUser(UserId);
            return View(roles);
        }

        public ActionResult ShowAllUsers()
        {
            return View(db.Users.ToList());
        }
    }
}