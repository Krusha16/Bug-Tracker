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

        public static List<Ticket> GetSortedTickets(List<Ticket> filteredTickets ,string sortBy)
        {
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
            return sortedTickets;
        }
    }
}