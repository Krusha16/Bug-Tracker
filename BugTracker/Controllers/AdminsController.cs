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
            return RedirectToAction("GetAllRolesForUser");
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
            MembershipHelper.RemoveUserFromRole(UserId, role);
            return RedirectToAction("GetAllRolesForUser");
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

        //Methods to create TicketPriority, TicketStatus, TicketType.

        public ActionResult CreateTicketPriority()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicketPriority([Bind(Include = "Id,Name")] TicketPriority ticketPriority)
        {
            if (ModelState.IsValid)
            {
                db.TicketPriorities.Add(ticketPriority);
                db.SaveChanges();
                return RedirectToAction("AllTickets", "Tickets");
            }
            return RedirectToAction("AllTickets", "Tickets");
        }

        public ActionResult CreateTicketStatus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicketStatus([Bind(Include = "Id,Name")] TicketStatus ticketstatus)
        {
            if (ModelState.IsValid)
            {
                db.TicketStatuses.Add(ticketstatus);
                db.SaveChanges();
                return RedirectToAction("AllTickets", "Tickets");
            }
            return RedirectToAction("AllTickets", "Tickets");
        }

        public ActionResult CreateTicketType()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicketType([Bind(Include = "Id,Name")] TicketType tickettype)
        {
            if (ModelState.IsValid)
            {
                db.TicketTypes.Add(tickettype);
                db.SaveChanges();
                return RedirectToAction("AllTickets", "Tickets");
            }
            return RedirectToAction("AllTickets", "Tickets");
        }
    }
}