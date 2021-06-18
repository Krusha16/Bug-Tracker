﻿using Microsoft.AspNet.Identity;
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

        public static void UpdateHistory(Ticket oldTicket,Ticket newTicket)
        {
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                CreateNewPriorityHistory(oldTicket, newTicket);
            }
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                CreateNewTypeHistory(oldTicket, newTicket);
            }
            if (oldTicket.Title != newTicket.Title)
            {
                CreateNewTitleHistory(oldTicket, newTicket);
            }
            if (oldTicket.Description != newTicket.Description)
            {
                CreateNewDescriptionHistory(oldTicket, newTicket);
            }
            db.SaveChanges();
        }

        public static void CreateNewTitleHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = newTicket,
                OldValue = oldTicket.Title,
                NewValue = newTicket.Title,
                Property = "Ticket Title"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }

        public static void CreateNewDescriptionHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = newTicket,
                OldValue = oldTicket.Description,
                NewValue = newTicket.Description,
                Property = "Ticket Description"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }

        public static void CreateNewPriorityHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = newTicket,
                OldValue = oldTicket.TicketPriority.Name,
                NewValue = newTicket.TicketPriority.Name,
                Property = "Ticket Priority"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }

        public static void CreateNewTypeHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = newTicket,
                OldValue = oldTicket.TicketType.Name,
                NewValue = newTicket.TicketType.Name,
                Property = "Ticket Type"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }

        public static void CreateNewDeveloperHistory(Ticket ticket, String UserId)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var oldUser = db.Users.Find(UserId);
            if(oldUser.Email == null)
            {
                oldUser.Email = "-";
            }
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = ticket,
                OldValue = ticket.AssignedToUser.Email,
                NewValue = oldUser.Email,
                Property = "Assigned Developer"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }

        public static void CreateNewStatusHistory(Ticket ticket, int statusId)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var newStatus = db.TicketStatuses.Find(statusId);
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = ticket,
                OldValue = ticket.TicketStatus.Name,
                NewValue = newStatus.Name,
                Property = "Ticket Status"
            };
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();
        }
    }
}