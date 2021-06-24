using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static void AddUserToProjectUsers(Project project, string UserId)
        {
            ProjectUser projectUser = new ProjectUser();
            projectUser.UserId = UserId;
            projectUser.ProjectId = project.Id;
            var users = project.ProjectUsers.Any(p => p.UserId == UserId);
            if (!users)
            {
                db.ProjectUsers.Add(projectUser);
            }
            db.SaveChanges();
        }

        //public void UpdateResolvedPercentages()
        //{
        //    foreach (var project in db.Projects.ToList())
        //    {
        //        var resolved = project.Tickets.Where(t => t.TicketStatus.Name == "RESOLVED").ToList().Count;
        //        if (resolved != 0)
        //        {
        //            project.ResolvedPercentage = (resolved / project.Tickets.Count) * 100;
        //            db.SaveChanges();
        //        }
        //    }
        //}
    }
}