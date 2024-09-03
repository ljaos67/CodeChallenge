using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//<summary>
// Employee database context, includes employee data and employee compensation data. 
//<summary>
namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary key for Compensation
            modelBuilder.Entity<Compensation>()
                .HasKey(c => c.CompensationId);

            // Define primary key for Employee
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);

            // Define the relationship between Compensation and Employee
            modelBuilder.Entity<Compensation>()
                .HasOne(c => c.Employee)       // Compensation has one Employee
                .WithMany()                    // Employee has many compensations (or WithOne() if no navigation back)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
