using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult AddTicketComment(int ticketId, string content)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                TicketComment ticketComment = new TicketComment
                {
                    Comment = content,
                    Created = DateTime.Now,
                    TicketId = ticketId,
                    UserId = userId
                };
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                TicketNotificationHelper.AddNotificationForNewProperty(ticketId, userId, "comment");
            }
            return RedirectToAction("AllTickets","Tickets");
        }
    }
}