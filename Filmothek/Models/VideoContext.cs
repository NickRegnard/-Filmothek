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

        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerHistory> CustomerHistory { get; set; }
        public DbSet<Moderator> Moderator { get; set; }
        public DbSet<Moderatorhistory> ModeratorHistory { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet <PaymentMethod> PaymentMethod {get; set;}

    }
}


    
