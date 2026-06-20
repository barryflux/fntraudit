using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FntrAudit.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
             : base(options)
        {
        }
        public DbSet<User> Aspnetusers { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Activity> Activities { get; set; }
      //  public DbSet<SocieteUser> SocieteUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("AspNetUsers");

                entity.HasKey(x => x.id);

            });
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");

                entity.HasKey(x => x.id);

            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("Activite");

                entity.HasKey(x => x.id);

            });
        }

    }
}
