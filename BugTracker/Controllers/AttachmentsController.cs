﻿using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class AttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult AddAttachmentToTicket(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAttachmentToTicket(int id, TicketAttachment attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                attachment.TicketId = id;
                attachment.Created = DateTime.Now;
                attachment.UserId = userId;
                attachment.FilePath = Server.MapPath("~/AttachedFiles");
                string partialFileName = Path.GetFileName(file.FileName);
                attachment.FileUrl = Path.Combine(attachment.FilePath, partialFileName);
                file.SaveAs(attachment.FilePath + partialFileName);
                TicketAttachmentHelper.AddAttachmentToTicket(attachment);
                TicketNotificationHelper.AddNotificationForNewProperty(id, userId, "attachment");
            }
            return RedirectToAction("AllTickets", "Tickets");
        }

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        public ActionResult EditAttachment(int id)
        {
            var attachment = db.TicketAttachments.Find(id);
            return View(attachment);
        }

        [HttpPost]
        public ActionResult EditAttachment(int id, TicketAttachment attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var oldAttachment = db.TicketAttachments.Find(id);
                if (file == null)
                {
                    oldAttachment.Description = attachment.Description;
                    db.Entry(oldAttachment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    attachment.TicketId = oldAttachment.TicketId;
                    attachment.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    attachment.Created = DateTime.Now;
                    attachment.FilePath = Server.MapPath("~/AttachedFiles");
                    string partialFileName = Path.GetFileName(file.FileName);
                    attachment.FileUrl = Path.Combine(attachment.FilePath, partialFileName);
                    file.SaveAs(attachment.FileUrl);
                    TicketAttachmentHelper.AddAttachmentToTicket(attachment);
                    TicketAttachmentHelper.DeleteAttachmentFromTicket(id);
                }
                return RedirectToAction("AllTickets", "Tickets");
            }
            return View(attachment);
        }

        [Authorize(Roles = "Project Manager, Submitter, Admin, Developer")]
        [HttpPost]
        public ActionResult DeleteAttachment(int id)
        {
            TicketAttachmentHelper.DeleteAttachmentFromTicket(id);
            return RedirectToAction("AllTickets", "Tickets");
        }
    }
}