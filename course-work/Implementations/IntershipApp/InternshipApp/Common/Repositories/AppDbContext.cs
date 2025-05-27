using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Common.Models; 
using Common.Entities;

namespace Common.Repositories
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Internship> Internships { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.University)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Degree)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegistrationDate)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });



            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ContactEmail)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.HeadquartersAddress)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CompanyPhone)              
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.EstablishedDate)
                    .IsRequired();

                entity.Property(e => e.IsVerified)
                    .IsRequired();
            });



            modelBuilder.Entity<Internship>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.Salary)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.ApplicationDate)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.Property(e => e.MotivationLetter)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.HasPortfolio)
                    .IsRequired();

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Internship)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.InternshipId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
