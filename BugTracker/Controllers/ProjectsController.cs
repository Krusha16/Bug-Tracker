using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ViewBag.Roles = MembershipHelper.GetAllRolesOfUser(userId);
            return View("~/Views/Projects/AllProjects.cshtml",db.Projects.ToList());
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AllProjects()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var filteredProjects = db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId));
            UpdateResolvedPercentages();
            ViewBag.Roles = MembershipHelper.GetAllRolesOfUser(userId);
            return View(filteredProjects.ToList());
        }

        public void UpdateResolvedPercentages()
        {
            foreach(var project in db.Projects.ToList())
            {
                var resolved = project.Tickets.Where(t => t.TicketStatus.Name == "RESOLVED").ToList().Count;
                if(resolved != 0)
                {
                    var percentage = (resolved / project.Tickets.Count) * 100;
                }
            }
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUserToProject()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Project Manager"))
            {
                var filteredProjects = db.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId));
                ViewBag.projectId = new SelectList(filteredProjects.ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.projectId = new SelectList(db.Projects.ToList(), "Id", "Name");
            }
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            return View();
        }

        [HttpPost]
        public ActionResult AssignUserToProject(string UserId, int projectId)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Project project = db.Projects.Find(projectId);
            ProjectHelper.AddUserToProjectUsers(project, UserId);
            db.SaveChanges();
            return RedirectToAction("AllProjects");
        }

        

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult UnAssignUserFromProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            List<ApplicationUser> projectUsers = project.ProjectUsers.Select(p => p.User).ToList();
            ViewBag.UserId = new SelectList(projectUsers, "Id", "Email");
            return View();
        }

        [HttpPost]
        public ActionResult UnAssignUserFromProject(int projectId, string UserId)
        {
            Project project = db.Projects.Find(projectId);
            List<ApplicationUser> projectUsers = project.ProjectUsers.Select(p => p.User).ToList();
            ViewBag.UserId = new SelectList(projectUsers, "Id", "Email");
            ProjectUser projectUser = db.ProjectUsers.FirstOrDefault(p => p.ProjectId == projectId && p.UserId == UserId);
            db.ProjectUsers.Remove(projectUser);
            db.SaveChanges();
            return RedirectToAction("AllProjects");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Create()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (MembershipHelper.CheckIfUserIsInRole(userId, "Admin"))
            {
                ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project, string UserId)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                ProjectUser projectUser = new ProjectUser();
                if (MembershipHelper.CheckIfUserIsInRole(currentUserId, "Project Manager"))
                {
                    projectUser.UserId = currentUserId;
                }
                else
                {
                    projectUser.UserId = UserId;
                }
                projectUser.Project = project;
                project.ProjectUsers.Add(projectUser);
                db.Projects.Add(project);
                db.SaveChanges();
            }
            return RedirectToAction("AllProjects");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("AllProjects");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("AllProjects");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
