using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            //SendEmail("krushapatel567@gmail.com", newNotification.Content);
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

        public static void AddNotificationForNewProperty(int ticketId, string UserId, string property)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            ApplicationUser applicationUser = db.Users.Find(UserId);
            TicketNotification newNotification = new TicketNotification();
            newNotification.UserId = applicationUser.Id;
            newNotification.TicketId = ticketId;
            newNotification.Content = "New " + property + " is added to the ticket - " + ticket.Title;
            db.TicketNotifications.Add(newNotification);
            db.SaveChanges();
        }

        public static void SendEmail(string receiverId, string content)
        {
            using (MailMessage mm = new MailMessage("krushampatel@yahoo.com", receiverId))
            {
                mm.Subject = "Update in ticket";
                mm.Body = content;
                mm.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential("krushampatel@yahoo.com", "Kv@16101995");
                    smtp.EnableSsl = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }
    }
}