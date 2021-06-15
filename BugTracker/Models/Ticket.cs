using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.TicketNotifications = new HashSet<TicketNotification>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey("OwnerUser")]
        public string OwnerUserId { get; set; }

        [ForeignKey("AssignedToUser")]
        public string AssignedToUserId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? TicketPriorityId { get; set; }
        public int? TicketStatusId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
    }
}