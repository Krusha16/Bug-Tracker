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
    public class TicketsControllerTest
    {
        [TestMethod]
        public void TestMethod_AllTickets_returns_List()
        {
            var context = new FakeApplicationDbContext
            {
                Tickets =
                {
                    new Ticket { Id = 1, Title = "AAA", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T1"},
                    new Ticket { Id = 2, Title = "BBB", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T2"},
                    new Ticket { Id = 3, Title = "CCC", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T3"},
                }
            };

            var controller = new TicketsController(context);
            var result = controller.AllTickets(1) as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(PagedList<Ticket>));
            var tickets = (IEnumerable<Ticket>)result.ViewData.Model;
            Assert.AreEqual("AAA", tickets.ToArray().ElementAt(0).Title);
            Assert.AreEqual("BBB", tickets.ToArray().ElementAt(1).Title);
            Assert.AreEqual("CCC", tickets.ToArray().ElementAt(2).Title);
        }

        [TestMethod]
        public void TestMethod_AssignDeveloperToTicket_SetUserTo_Ticket()
        {
            var context = new FakeApplicationDbContext
            {
                Tickets =
                {
                    new Ticket { Id = 1, Title = "AAA", OwnerUserId = "111", AssignedToUserId = "", Created = DateTime.Now, ProjectId = 1, Description = "T1"},
                    new Ticket { Id = 2, Title = "BBB", OwnerUserId = "111", AssignedToUserId = "", Created = DateTime.Now, ProjectId = 1, Description = "T2"},
                    new Ticket { Id = 3, Title = "CCC", OwnerUserId = "111", AssignedToUserId = "", Created = DateTime.Now, ProjectId = 1, Description = "T3"},
                },
                Users =
                {
                    new ApplicationUser { Id = "222" ,Tickets = {new Ticket { Id = 4, Title = "AAA", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T1"},
                    new Ticket { Id = 5, Title = "BBB", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T2"},
                    new Ticket { Id = 6, Title = "CCC", OwnerUserId = "111", Created = DateTime.Now, ProjectId = 1, Description = "T3"}, } }
                }
            };

            var controller = new TicketsController(context);
            var result = controller.AssignDeveloperToTicket(1, "222") as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Ticket>));
            var tickets = (IEnumerable<Ticket>)result.ViewData.Model;
            Assert.AreEqual("222", tickets.ToArray().ElementAt(0).AssignedToUserId);
        }
    }
}
