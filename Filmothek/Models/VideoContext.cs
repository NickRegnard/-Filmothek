using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class VideoContext : DbContext
    {
        public VideoContext(DbContextOptions<VideoContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PaymentMethod>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CustomerHistory>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Moderator>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ModeratorHistory>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Movie>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            //----------------------------------------------------------------
            modelBuilder.Entity<Customer>()
               .HasOne(pt => pt.PaymentMethod)
               .WithOne(p => p.Customer)
               .HasForeignKey<PaymentMethod>(s => s.CustomerId);
            //----------------------------------------------------------------
            modelBuilder.Entity<CustomerHistory>()
                .HasOne(pt => pt.Customer)
                .WithMany(p => p.CustomerHistory)
                .HasForeignKey(pt => pt.CustomerId);

            modelBuilder.Entity<CustomerHistory>()
                .HasOne(pt => pt.Movie)
                .WithMany(t => t.CustomerHistory)
                .HasForeignKey(pt => pt.MovieId);
            //----------------------------------------------------------------
            modelBuilder.Entity<ModeratorHistory>()
                .HasOne(pt => pt.Moderator)
                .WithMany(t => t.ModeratorHistory)
                .HasForeignKey(pt => pt.ModeratorId);
            //----------------------------------------------------------------
        }



        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerHistory> CustomerHistory { get; set; }
        public DbSet<Moderator> Moderator { get; set; }
        public DbSet<ModeratorHistory> ModeratorHistory { get; set; }
        //public DbSet<Admin> Admin { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }
    }
}


    
