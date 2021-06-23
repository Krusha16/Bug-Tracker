using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketNotificationHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static void AddNotificationForDeveloperUpdate(Ticket ticket, string UserId)
        {
            ApplicationUser applicationUser = db.Users.Find(UserId);
            TicketNotification newNotification = new TicketNotification();
            newNotification.UserId = applicationUser.Id;
            newNotification.TicketId = ticket.Id;
            newNotification.Content = "You are assigned to a new ticket - " + ticket.Title;
            db.TicketNotifications.Add(newNotification);
            db.SaveChanges();
        }

        public static void AddNotificationForTicketUpdate(Ticket ticket, string UserId, string editedField)
        {
            ApplicationUser applicationUser = db.Users.Find(UserId);
            TicketNotification newNotification = new TicketNotification();
            newNotification.UserId = applicationUser.Id;
            newNotification.TicketId = ticket.Id;
            newNotification.Content = "The " + editedField + " is updated for the ticket - " + ticket.Title;
            db.TicketNotifications.Add(newNotification);
            db.SaveChanges();
        }
    }
}