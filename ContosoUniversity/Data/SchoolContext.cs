using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    // NOTE: SchoolContext inherits from Microsoft.EntityFrameworkCore.DbContext; it is the databse context class. 
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        //NOTE: You specify which entities are included in the data model and customize certain Entity behavior. 

        
        //NOTE: This code creates a DbSet property for each entity set. An entity set corresponds to a database table and an entity corresponds to a row in the table.
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        //NOTE: When the database is created, EF creates tables that have names the same as the DbSet property names. Property names for collections are typically plural, however, table names are on a case-by-case basis. The code below overrides plural table names and makes them singular instead:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
        }
    }
}

//NOTE: The SchoolContext must be registered as a service in Startup.cs. The EF database context is a service; these must be registered with dependency injection during application startup. Components that require these services (such as MVC controllers) are provided them via constructor parameters. 