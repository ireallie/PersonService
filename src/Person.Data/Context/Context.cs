using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Person.Data.Entities
{
    public class Context : DbContext
    {
        #region properties
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }

        #endregion

        public Context(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Person>()
                .Property(p => p.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(p => p.LastName)
                .HasMaxLength(100)
                .IsRequired();


            modelBuilder.Entity<Address>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(a => a.AddressLine)
                .HasMaxLength(100)
                .IsRequired();

        }
    }
}
