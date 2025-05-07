using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Common.Entities;

namespace Common.Repositories
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Internship> Internships { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }

        // Настроим поведение моделей, если нужно
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Дополнительные конфигурации для сущностей (если надо)
        }

        // Устанавливаем подключение к базе данных
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Убедись, что указываешь свою строку подключения
                optionsBuilder.UseSqlServer("Server=localhost;Database=InternshipAppDb;Integrated Security=True;TrustServerCertificate=True;");
            }
        }
    }
}
