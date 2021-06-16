using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AllTickets()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.userID = userId;
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Submitter"))
            {
                ViewBag.Role = "Submitter";
            }
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Developer"))
            {
                ViewBag.Role = "Developer";
            }
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Project Manager"))
            {
                ViewBag.Role = "Project Manager";
            }
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Admin"))
            {
                ViewBag.Role = "Admin";
            }
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                ticket.OwnerUserId = userId;
                ticket.Created = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
            }
            return RedirectToAction("AllTickets");
        }

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,OwnerUserId,AssignedToUserId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("AllTickets");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignDeveloperToTicket(int id)
        {
            List<ApplicationUser> developers = new List<ApplicationUser>();
            foreach (var user in db.Users.ToList())
            {
                if (MembershipHelper.CheckIfUserIsInRole(user.Id, "Developer"))
                {
                    developers.Add(user);
                }
            }
            ViewBag.UserId = new SelectList(developers, "Id", "Email");
            return View();
        }

        [HttpPost]
        public ActionResult AssignDeveloperToTicket(int id, string UserId)
        {
            Ticket ticket = db.Tickets.Find(id);
            ticket.AssignedToUserId = UserId;
            db.SaveChanges();
            return RedirectToAction("AllTickets");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult UpdateStatus(int id)
        {
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, int statusId)
        {
            Ticket ticket = db.Tickets.Find(id);
            ticket.TicketStatusId = statusId;
            db.SaveChanges();
            return RedirectToAction("AllTickets");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("AllTickets");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        

        [Authorize(Roles = "Developer, Project Manager")]
        public ActionResult AllTicketsByProjectForDevelopersAndProjectManagers()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredProjects = db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId));
            //var tickets = filteredProjects.Select(t => t.Tickets).AsQueryable(); 
            return View(filteredProjects.ToList());

        }

        [Authorize(Roles = "Submitter")]
        public ActionResult AllTicktesForSubmitters()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTickets = db.Tickets.Where(t => t.OwnerUserId == userId);

            return View("~/Views/Tickets/AllTickets.cshtml", filteredTickets.ToList());
        }
    }
}
