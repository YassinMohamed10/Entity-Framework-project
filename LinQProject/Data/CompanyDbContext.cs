using LinQProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinQProject.Data
{
    internal class CompanyDbContext : DbContext
    {

        public CompanyDbContext()
        {
        }

        public CompanyDbContext(DbContextOptions<CompanyDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<EmployeeProject> EmployeeProjects { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //string str = "Data Source=D:\\\\C#Projects\\\\Backend\\\\ConsoleApp1\\\\company4.db";
            string str = "Data Source=(localdb)\\ProjectModels;Initial Catalog=CompanyDB4;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            optionsBuilder.UseSqlite(str);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                entity.HasIndex(e => e.DepartmentId, "IX_Employees_DepartmentID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Departments");
            });

            modelBuilder.Entity<EmployeeProject>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.ProjectId });

                entity.HasIndex(e => e.ProjectId, "IX_EmployeeProjects_ProjectID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Role).HasMaxLength(50);

                entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeProjects)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeProjects_Employees");

                entity.HasOne(d => d.Project).WithMany(p => p.EmployeeProjects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeProjects_Projects");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.ProjectId);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Name).HasMaxLength(100);
            });
        }

    }


}

