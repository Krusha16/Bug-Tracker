using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.ProjectUsers = new HashSet<ProjectUser>();
            this.Tickets = new HashSet<Ticket>();
            this.TicketNotifications = new HashSet<TicketNotification>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
        }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public interface IApplicationDbContext
    {
        IDbSet<Project> Projects { get; }
        IDbSet<ProjectUser> ProjectUsers { get; }
        IDbSet<Ticket> Tickets { get; }
        IDbSet<TicketAttachment> TicketAttachments { get; }
        IDbSet<TicketComment> TicketComments { get; }
        IDbSet<TicketHistory> TicketHistories { get; }
        IDbSet<TicketNotification> TicketNotifications { get; }
        IDbSet<TicketPriority> TicketPriorities { get; }
        IDbSet<TicketStatus> TicketStatuses { get; }
        IDbSet<TicketType> TicketTypes { get; }
        IDbSet<ApplicationUser> Users { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }

    public class FakeApplicationDbContext : IApplicationDbContext
    {
        public FakeApplicationDbContext() 
        {
            this.Projects = new FakeProjecteSet();
            this.ProjectUsers = new FakeProjectUserSet();
            this.Tickets = new FakeTicketSet();
            this.TicketAttachments = new FakeTicketAttachmentSet();
            this.Users = new FakeApplicationUserSet();
        }
        public IDbSet<Project> Projects { get; private set; }
        public IDbSet<ProjectUser> ProjectUsers { get; private set; }
        public IDbSet<Ticket> Tickets { get; private set; }
        public IDbSet<TicketAttachment> TicketAttachments { get; private set; }
        public IDbSet<TicketComment> TicketComments { get; private set; }
        public IDbSet<TicketHistory> TicketHistories { get; private set; }
        public IDbSet<TicketNotification> TicketNotifications { get; private set; }
        public IDbSet<TicketPriority> TicketPriorities { get; private set; }
        public IDbSet<TicketStatus> TicketStatuses { get; private set; }
        public IDbSet<TicketType> TicketTypes { get; private set; }
        public IDbSet<ApplicationUser> Users { get; private set; }
        public int SaveChanges() {
            return 0;
        }
        public DbEntityEntry Entry(object entity) { Entry(entity).State = EntityState.Modified; return (DbEntityEntry)entity; }
        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class { Entry(entity).State = EntityState.Modified; return entity as DbEntityEntry<TEntity>; }
    }
        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<ProjectUser> ProjectUsers { get; set; }
        public IDbSet<Ticket> Tickets { get; set; }
        public IDbSet<TicketAttachment> TicketAttachments { get; set; }
        public IDbSet<TicketComment> TicketComments { get; set; }
        public IDbSet<TicketHistory> TicketHistories { get; set; }
        public IDbSet<TicketNotification> TicketNotifications { get; set; }
        public IDbSet<TicketPriority> TicketPriorities { get; set; }
        public IDbSet<TicketStatus> TicketStatuses { get; set; }
        public IDbSet<TicketType> TicketTypes { get; set; }

        public ApplicationDbContext()
            : base("BugTrackerConnectionString", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}