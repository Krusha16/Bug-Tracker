using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketAttachmentHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        public static void AddAttachmentToTicket(TicketAttachment attachment)
        {
            db.TicketAttachments.Add(attachment);
            db.SaveChanges();
        }

        public static void DeleteAttachmentFromTicket(int id)
        {
            var attachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(attachment);
            File.Delete(attachment.FileUrl);
            db.SaveChanges();
        }
    }
}