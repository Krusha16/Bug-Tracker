using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistoryHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static List<TicketHistory> UpdateHistory(Ticket oldTicket, Ticket newTicket)
        {
            List<TicketHistory> newHistories = new List<TicketHistory>();
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                var newHistory = CreateNewPriorityHistory(oldTicket, newTicket);
                newHistories.Add(newHistory);
                TicketNotificationHelper.AddNotificationForTicketUpdate(oldTicket, oldTicket.AssignedToUserId,"ticket priority");
            }
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                var newHistory = CreateNewTypeHistory(oldTicket, newTicket);
                newHistories.Add(newHistory);
                TicketNotificationHelper.AddNotificationForTicketUpdate(oldTicket, oldTicket.AssignedToUserId, "ticket type");
            }
            if (oldTicket.Title != newTicket.Title)
            {
                var newHistory = CreateNewTitleHistory(oldTicket, newTicket);
                newHistories.Add(newHistory);
                TicketNotificationHelper.AddNotificationForTicketUpdate(oldTicket, oldTicket.AssignedToUserId, "ticket title");
            }
            if (oldTicket.Description != newTicket.Description)
            {
                var newHistory = CreateNewDescriptionHistory(oldTicket, newTicket);
                newHistories.Add(newHistory);
                TicketNotificationHelper.AddNotificationForTicketUpdate(oldTicket, oldTicket.AssignedToUserId, "ticket description");
            }
            return newHistories;
        }

        public static TicketHistory CreateNewTitleHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = oldTicket,
                OldValue = oldTicket.Title,
                NewValue = newTicket.Title,
                Property = "Ticket Title"
            };
            return newHistory;
        }

        public static TicketHistory CreateNewDescriptionHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = oldTicket,
                OldValue = oldTicket.Description,
                NewValue = newTicket.Description,
                Property = "Ticket Description"
            };
            return newHistory;
        }

        public static TicketHistory CreateNewPriorityHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = newTicket,
                OldValue = oldTicket.TicketPriority.Name,
                NewValue = db.TicketPriorities.Find(newTicket.TicketPriorityId).Name,
                Property = "Ticket Priority"
            };
            return newHistory;
        }

        public static TicketHistory CreateNewTypeHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = oldTicket,
                OldValue = oldTicket.TicketType.Name,
                NewValue = db.TicketTypes.Find(newTicket.TicketTypeId).Name,
                Property = "Ticket Type"
            };
            return newHistory;
        }

        public static TicketHistory CreateNewDeveloperHistory(Ticket ticket, String newUserId)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory();
            newHistory.UserId = userId;
            newHistory.Changed = DateTime.Now;
            newHistory.Ticket = ticket;
            if (ticket.AssignedToUser == null)
            {
                newHistory.OldValue = "-";
            }
            else
            {
                newHistory.OldValue = ticket.AssignedToUser.Email;
            }
            newHistory.NewValue = db.Users.Find(newUserId).Email;
            newHistory.Property = "Assigned Developer";
            TicketNotificationHelper.AddNotificationForDeveloperUpdate(ticket, newUserId);
            return newHistory;
        }

        public static TicketHistory CreateNewStatusHistory(Ticket ticket, int statusId)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            TicketHistory newHistory = new TicketHistory
            {
                UserId = userId,
                Changed = DateTime.Now,
                Ticket = ticket,
                OldValue = ticket.TicketStatus.Name,
                NewValue = db.TicketStatuses.Find(statusId).Name,
                Property = "Ticket Status"
            };
            TicketNotificationHelper.AddNotificationForTicketUpdate(ticket, ticket.AssignedToUserId,"ticket status");
            return newHistory;
        }
    }
}