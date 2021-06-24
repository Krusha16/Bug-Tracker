using BugTracker.Controllers;
using BugTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BugTrackerTest.Controllers
{
    [TestClass]
    public class ProjectsControllerTest
    {
        [TestMethod]
        public void TestMethod_AllProjects_returns_List()
        {
            var context = new FakeApplicationDbContext
            {
                Projects =
                 {
                     new Project { Id = 1, Name = "AAA"},
                     new Project { Id = 2, Name = "BBB"},
                     new Project { Id = 3, Name = "CCC"},
                 }
            };

            var controller = new ProjectsController(context);
            var result = controller.AllProjects() as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Project>));
            var projects = (IEnumerable<Project>)result.ViewData.Model;
            Assert.AreEqual("AAA", projects.ToArray().ElementAt(0).Name);
            Assert.AreEqual("BBB", projects.ToArray().ElementAt(1).Name);
            Assert.AreEqual("CCC", projects.ToArray().ElementAt(2).Name);
        }

        [TestMethod]
        public void TestMethod_AssignDeveloperToProject_SetUserTo_ProjectUsers()
        {
            var context = new FakeApplicationDbContext
            {
                Projects =
                 {
                     new Project { Id = 1, Name = "AAA", ProjectUsers = { }},
                     new Project { Id = 2, Name = "BBB", ProjectUsers = { }},
                     new Project { Id = 3, Name = "CCC", ProjectUsers = { }},
                 },
                Users =
                 {
                     new ApplicationUser { Id = "222" ,Tickets = {new Ticket { Id = 4, Title = "AAA", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T1"},
                     new Ticket { Id = 5, Title = "BBB", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T2"},
                     new Ticket { Id = 6, Title = "CCC", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T3"}, } }
                 }
            };

            var controller = new ProjectsController(context);
            var result = controller.AssignUserToProject("222", 1) as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Project>));
            var projects = (IEnumerable<Project>)result.ViewData.Model;
            Assert.AreEqual("222", projects.ToArray().ElementAt(0).ProjectUsers.ToArray().ElementAt(0).UserId);
        }
        [TestMethod]
        public void TestMethod_UnassignDeveloperToProject_SetUserTo_ProjectUsers()
        {
            var context = new FakeApplicationDbContext
            {

                Projects =
                {
                    new Project { Id = 1, Name = "AAA", ProjectUsers = { new ProjectUser { UserId = "111", ProjectId = 1 }}},
                    new Project { Id = 2, Name = "BBB", ProjectUsers = { }},
                    new Project { Id = 3, Name = "CCC", ProjectUsers = { }},
                }
            };

            var controller = new ProjectsController(context);
            var result = controller.UnAssignUserFromProject(1, "111") as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Project>));
            var projects = (IEnumerable<Project>)result.ViewData.Model;
            Assert.AreEqual(1, projects.ToArray().ElementAt(0).ProjectUsers.Count());
        }
    }
}
