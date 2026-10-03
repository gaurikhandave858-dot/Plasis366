using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;


namespace Plasis366.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Region> Regions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Taluka> Talukas { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<DesignRequirement> DesignRequirements { get; set; }
        public DbSet<ProjectAttachment> ProjectAttachments { get; set; }
        public DbSet<DesignProposal> DesignProposals { get; set; }
        public DbSet<ProjectStatusHistory> ProjectStatusHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Region>()
                .HasKey(x => x.RegionId);

            modelBuilder.Entity<Country>()
                .HasKey(x => x.CountryId);

            modelBuilder.Entity<State>()
                .HasKey(x => x.StateId);

            modelBuilder.Entity<District>()
                .HasKey(x => x.DistrictId);

            modelBuilder.Entity<Taluka>()
                .HasKey(x => x.TalukaId);

            modelBuilder.Entity<City>()
                .HasKey(x => x.CityId);

            modelBuilder.Entity<Tenant>()
                .HasKey(x => x.TenantId);

            modelBuilder.Entity<User>()
                .HasKey(x => x.UserId);

            modelBuilder.Entity<Role>()
                .HasKey(x => x.RoleId);

            modelBuilder.Entity<Permission>()
                .HasKey(x => x.PermissionId);

            modelBuilder.Entity<UserRole>()
                .HasKey(x => x.UserRoleId);

            modelBuilder.Entity<RolePermission>()
                .HasKey(x => x.RolePermissionId);

            modelBuilder.Entity<Customer>()
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<Project>()
                .HasKey(x => x.ProjectId);

            modelBuilder.Entity<Property>()
                .HasKey(x => x.PropertyId);

            modelBuilder.Entity<Room>()
                .HasKey(x => x.RoomId);

            modelBuilder.Entity<DesignRequirement>()
                .HasKey(x => x.DesignRequirementId);

            modelBuilder.Entity<ProjectAttachment>()
                .HasKey(x => x.ProjectAttachmentId);

            modelBuilder.Entity<DesignProposal>()
                .HasKey(x => x.DesignProposalId);

            modelBuilder.Entity<ProjectStatusHistory>()
                .HasKey(x => x.ProjectStatusHistoryId);

            modelBuilder.Entity<Notification>()
                .HasKey(x => x.NotificationId);

            modelBuilder.Entity<Country>()
                .HasOne(x => x.Region)
                .WithMany(x => x.Countries)
                .HasForeignKey(x => x.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<State>()
                .HasOne(x => x.Country)
                .WithMany(x => x.States)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<District>()
                .HasOne(x => x.State)
                .WithMany(x => x.Districts)
                .HasForeignKey(x => x.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Taluka>()
                .HasOne(x => x.District)
                .WithMany(x => x.Talukas)
                .HasForeignKey(x => x.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>()
                .HasOne(x => x.Taluka)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.TalukaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tenant>()
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(x => x.DesignerUser)
                .WithMany()
                .HasForeignKey(x => x.DesignerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.State)
                .WithMany()
                .HasForeignKey(x => x.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.District)
                .WithMany()
                .HasForeignKey(x => x.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.Taluka)
                .WithMany()
                .HasForeignKey(x => x.TalukaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(x => x.Property)
                .WithMany(x => x.Rooms)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DesignRequirement>()
                .HasOne(x => x.Project)
                .WithMany(x => x.DesignRequirements)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DesignRequirement>()
                .HasOne(x => x.Room)
                .WithMany(x => x.DesignRequirements)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectAttachment>()
                .HasOne(x => x.Project)
                .WithMany(x => x.ProjectAttachments)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DesignProposal>()
                .HasOne(x => x.Project)
                .WithMany(x => x.DesignProposals)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DesignProposal>()
                .HasOne(x => x.SubmittedByUser)
                .WithMany()
                .HasForeignKey(x => x.SubmittedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectStatusHistory>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectStatusHistory>()
                .HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditEntity.IsActive))
                        .HasDefaultValue(true);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditEntity.VersionNumber))
                        .HasDefaultValue(1);
                }
            }

            modelBuilder.Entity<Region>()
                .Property(x => x.RegionName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Country>()
                .Property(x => x.CountryName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<State>()
                .Property(x => x.StateName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<District>()
                .Property(x => x.DistrictName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Taluka>()
                .Property(x => x.TalukaName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<City>()
                .Property(x => x.CityName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Tenant>()
                .Property(x => x.TenantName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Tenant>()
                .Property(x => x.TenantCode)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.UserName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(x => x.RoleName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Permission>()
                .Property(x => x.PermissionName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(x => x.ProjectName)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(x => x.ProjectCode)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Tenant>()
                .HasIndex(x => x.TenantCode)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => new { x.TenantId, x.UserName })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => new { x.TenantId, x.Email })
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(x => new { x.TenantId, x.RoleName })
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(x => new { x.TenantId, x.ProjectCode })
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasIndex(x => new { x.UserId, x.RoleId })
                .IsUnique();

            modelBuilder.Entity<RolePermission>()
                .HasIndex(x => new { x.RoleId, x.PermissionId })
                .IsUnique();
        }
    }
}