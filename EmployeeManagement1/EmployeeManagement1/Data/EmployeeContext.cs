using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

        public DbSet<Models.Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
               .HasIndex(employee => employee.Email)
               .IsUnique();
        }
    }
}
