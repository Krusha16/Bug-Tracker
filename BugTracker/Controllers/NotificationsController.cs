using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            return View(applicationUser.TicketNotifications.ToList());
        }

        public ActionResult OpenNotification(int id)
        {
            TicketNotification notification = db.TicketNotifications.Find(id);
            return View(notification);
        }
    }
}