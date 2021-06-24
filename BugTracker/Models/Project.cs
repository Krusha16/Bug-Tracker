using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            this.ProjectUsers = new HashSet<ProjectUser>();
            this.Tickets = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        //public int? ResolvedPercentage { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}