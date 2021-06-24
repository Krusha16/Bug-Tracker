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
            db.SaveChanges();
            SendEmail(applicationUser.Email, newNotification.Content);
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
            SendEmail(applicationUser.Email, newNotification.Content);
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
            SendEmail(applicationUser.Email, newNotification.Content);
        }

        public static void SendEmail(string receiverId, string content)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(receiverId);
            mail.From = new MailAddress("krushapatel567@gmail.com");
            mail.Subject = "New Notiofication sent by Bug-Tracker Project";
            mail.Body = content;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("krushapatel567@gmail.com", "");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
