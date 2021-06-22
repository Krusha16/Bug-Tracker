using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static List<Ticket> GetFilteredTickets(List<string> roles)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTickets = new List<Ticket>();
            if (roles.Contains("Submitter"))
            {
                var submitterTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                filteredTickets.AddRange(submitterTickets);
            }
            if (roles.Contains("Developer"))
            {
                var developerTickets = db.Tickets.Where(t => t.AssignedToUserId == userId);
                filteredTickets.AddRange(developerTickets);
            }
            if (roles.Contains("Project Manager"))
            {
                var managerProjects = db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId)).ToList();
                var managerTickets = new List<Ticket>();
                foreach (var project in managerProjects)
                {
                    managerTickets.AddRange(project.Tickets.ToList());
                }
                filteredTickets.AddRange(managerTickets);
            }
            if (roles.Contains("Admin"))
            {
                filteredTickets = db.Tickets.ToList();
            }
            var tickets = filteredTickets.ToHashSet();
            return tickets.ToList();
        }

        public static void AddAttachmentToTicket(TicketAttachment attachment)
        {
            db.TicketAttachments.Add(attachment);
            db.SaveChanges();
        }

        public static void DeleteAttachmentFromTicket(int id)
        {
            var attachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(attachment);
            db.SaveChanges();
        }


    }
}