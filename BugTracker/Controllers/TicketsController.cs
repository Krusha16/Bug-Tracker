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
using System.Web;
using System.Globalization;
using System.IO;

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
            ApplicationUser user = db.Users.Find(userId);
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            ViewBag.NotificationCount = user.TicketNotifications.Count;
            var filteredTickets = TicketHelper.GetFilteredTickets(roles);
            return View(filteredTickets.ToPagedList(i ?? 1, 10));
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult SortTickets(string sortBy, int? i)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            var filteredTickets = TicketHelper.GetFilteredTickets(roles).ToList();
            var sortedTickets = TicketHelper.GetSortedTickets(filteredTickets, sortBy);
            return View("~/Views/Tickets/AllTickets.cshtml", sortedTickets.ToPagedList(i ?? 1, 10));
        }

        public ActionResult SearchTickets(string option, string searchBy, int? i)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            var filteredTickets = TicketHelper.GetFilteredTickets(roles).ToList();
            var searchedTickets = new List<Ticket>();
            ViewBag.option = option;
            searchBy = searchBy.ToLower();
            ViewBag.searchBy = searchBy;
            ModelState["searchBy"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            switch (option)
            {
                case "Project":
                    searchedTickets = filteredTickets.Where(t => t.Project.Name.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Priority":
                    searchedTickets = filteredTickets.Where(t => t.TicketPriority.Name.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Status":
                    searchedTickets = filteredTickets.Where(t => t.TicketStatus.Name.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Type":
                    searchedTickets = filteredTickets.Where(t => t.TicketType.Name.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Title":
                    searchedTickets = filteredTickets.Where(t => t.Title.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Description":
                    searchedTickets = filteredTickets.Where(t => t.Description.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Submitter":
                    searchedTickets = filteredTickets.Where(t => t.OwnerUser.Email.ToLower().Contains(searchBy)).ToList();
                    break;

                case "Developer":
                    searchedTickets = filteredTickets.Where(t => t.AssignedToUser.Email.ToLower().Contains(searchBy)).ToList();
                    break;

                case "AllTickets":
                    searchedTickets = filteredTickets;
                    break;

                default:
                    Console.WriteLine("Default searching case");
                    break;
            }

            return View("~/Views/Tickets/AllTickets.cshtml", searchedTickets.ToPagedList(i ?? 1, 10));
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
            var newHistories = TicketHistoryHelper.UpdateHistory(oldTicket, ticket);
            foreach (var history in newHistories)
            {
                db.TicketHistories.Add(history);
            }
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
            var newHistory = TicketHistoryHelper.CreateNewDeveloperHistory(ticket, UserId);
            db.TicketHistories.Add(newHistory);
            ticket.AssignedToUserId = UserId;
            ProjectHelper.AddUserToProjectUsers(ticket.Project, UserId);
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
            var newHistory = TicketHistoryHelper.CreateNewStatusHistory(ticket, statusId);
            db.TicketHistories.Add(newHistory);
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
