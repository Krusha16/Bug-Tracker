using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
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
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
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

        public ActionResult SearchTickets(string option, string searchBy, int? i)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = MembershipHelper.GetAllRolesOfUser(userId);
            ViewBag.Roles = roles;
            var filteredTickets = TicketHelper.GetFilteredTickets(roles).ToList();
            var searchedTickets = new List<Ticket>();
            ViewBag.option = option;
            ViewBag.searchBy = searchBy;
            ModelState["searchBy"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            switch (option)
            {
                case "Project":
                    searchedTickets = filteredTickets.Where(t => t.Project.Name.Contains(searchBy)).ToList();
                    break;

                case "Priority":
                    searchedTickets = filteredTickets.Where(t => t.TicketPriority.Name.Contains(searchBy)).ToList();
                    break;

                case "Status":
                    searchedTickets = filteredTickets.Where(t => t.TicketStatus.Name.Contains(searchBy)).ToList();
                    break;

                case "Type":
                    searchedTickets = filteredTickets.Where(t => t.TicketType.Name.Contains(searchBy)).ToList();
                    break;

                case "Title":
                    searchedTickets = filteredTickets.Where(t => t.Title.Contains(searchBy)).ToList();
                    break;

                case "Description":
                    searchedTickets = filteredTickets.Where(t => t.Description.Contains(searchBy)).ToList();
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
            //TicketHistoryHelper.UpdateHistory(oldTicket, ticket);
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
            //TicketHistoryHelper.CreateNewDeveloperHistory(ticket, UserId);
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
            //TicketHistoryHelper.CreateNewStatusHistory(ticket, statusId);
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

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult AddAttachmentToTicket(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAttachmentToTicket(int id, TicketAttachment attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                attachment.TicketId = id;
                attachment.Created = DateTime.Now;
                attachment.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                attachment.FilePath = Server.MapPath("~/App_Data/AttachedFiles");
                string partialFileName = Path.GetFileName(file.FileName);
                attachment.FileUrl = Path.Combine(attachment.FilePath, partialFileName);
                file.SaveAs(attachment.FileUrl);
                TicketHelper.AddAttachmentToTicket(attachment);
                return RedirectToAction("AllTickets");
            }
            return View(attachment);
        }

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult EditAttachment(int id)
        {
            var attachment = db.TicketAttachments.Find(id);
            return View("AddAttachmentToTicket", attachment);
        }
        [HttpPost]
        public ActionResult EditAttachment(int id, TicketAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var oldAttachment = db.TicketAttachments.Find(id);
                attachment.TicketId = oldAttachment.TicketId;
                attachment.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                TicketHelper.DeleteAttachmentFromTicket(id);
                TicketHelper.AddAttachmentToTicket(attachment);
                return RedirectToAction("AllTickets");
            }
            return View("AddAttachmentToTicket", attachment);
        }

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult DeleteAttachment(int id)
        {
            TicketHelper.DeleteAttachmentFromTicket(id);
            return RedirectToAction("Details");
        }
    }
}
