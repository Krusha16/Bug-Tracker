using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}