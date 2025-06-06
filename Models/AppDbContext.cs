using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    internal class AppDbContext:DbContext
    {

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public virtual DbSet<Availability> Availabilitys { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Sale> Sales { get; set; }

        public virtual DbSet<Store> Stores { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

            => optionsBuilder.UseMySql("server=localhost;port=3306;database=your_db_name;user=root;password=your_password;",
            new MySqlServerVersion(new Version(8, 0, 3)));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка таблицы Product
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p=>p.Id);

            modelBuilder.Entity<Product>()
                .Property(p => p.ExpiryDate)
                .HasColumnName("Date");

            // Настройка таблицы Availability (композитный ключ)
            modelBuilder.Entity<Availability>()
                .ToTable("Availability")
                .HasKey(a => new { a.StoreId, a.ProductId });

            modelBuilder.Entity<Availability>()
                .Property(a => a.Quantity)
                .HasColumnName("Availability");

            // Настройка таблицы Sale (композитный ключ)
            modelBuilder.Entity<Sale>()
                .ToTable("Sales")
                .HasKey(s => new { s.CustomerId, s.ProductId, s.SaleDate });

            // Остальные таблицы
            modelBuilder.Entity<Customer>()
                .ToTable("Customers")
                .HasKey(c=>c.Id);

            modelBuilder.Entity<Store>()
                .ToTable("Stores")
                .HasKey(s=>s.Id);
        }
    }
}
