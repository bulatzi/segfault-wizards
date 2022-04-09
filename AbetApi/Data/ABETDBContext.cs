using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using AbetApi.EFModels;

namespace AbetApi.Data
{
    // This class is used to generate a database with entity framework, and connect to that database.
    public class ABETDBContext : DbContext
    {
        // All of these DbSet collections are used to create database tables with entity framework
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseOutcome> CourseOutcomes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<MajorOutcome> MajorOutcomes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentOutcomesCompleted> StudentOutcomesCompleted {get;set;}

        // This function is used to select a connection string for your database.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=abetrepo;user=root;password=1234");
        }

        // This function is used to add database constraints to data in tables
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.EUID).IsUnique();
                
            });

            builder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                
                //entity.HasOne<User>().WithMany<Role>();
            });
        }
        
        // This is here for entity framework, so it can generate database tables
        public ABETDBContext() : base()
        {
            //Intentionally left blank
        }

    }

    // This is a custom function that adds a clear function to all DbSet classes
    public static class EntityExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
