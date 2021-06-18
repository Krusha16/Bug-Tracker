using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            return RedirectToAction("AllTickets");
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AllTickets(int? i)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            return View(TicketHelper.GetFilteredTickets(roles).ToPagedList(i ?? 1, 10));
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult SortTickets(string sortBy, int? i)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            var filteredTickets = TicketHelper.GetFilteredTickets(roles).ToList();
            var sortedTickets = new List<Ticket>();
            switch (sortBy)
            {
                case "type":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.TicketTypeId).ToList();
                    break;
                case "status":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.TicketStatusId).ToList();
                    break;
                case "priority":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.TicketPriorityId).ToList();
                    break;
                case "creation":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.Created).ToList();
                    break;
                case "update":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.Updated).ToList();
                    break;
                case "title":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.Title).ToList();
                    break;
                case "owner":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.OwnerUserId).ToList();
                    break;
                case "developer":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.AssignedToUserId).ToList();
                    break;
                case "project":
                    sortedTickets = filteredTickets.OrderByDescending(p => p.ProjectId).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            return View("~/Views/Tickets/AllTickets.cshtml",sortedTickets.ToPagedList(i ?? 1, 10));
        }

        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var submitterProjects = db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId)).ToList();
            ViewBag.ProjectId = new SelectList(submitterProjects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
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
                ticket.TicketStatus = db.TicketStatuses.FirstOrDefault();
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
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,OwnerUserId,AssignedToUserId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket, int id)
        {
            Ticket oldTicket = db.Tickets.Find(id);
            TicketHelper.UpdateHistory(oldTicket, ticket);
            db.SaveChanges();
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
            TicketHelper.CreateNewDeveloperHistory(ticket, UserId);
            ticket.AssignedToUserId = UserId;
            ProjectHelper.AddUserToProjectUsers(ticket.Project, UserId);
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }
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
            TicketHelper.CreateNewStatusHistory(ticket, statusId);
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
    }
}
