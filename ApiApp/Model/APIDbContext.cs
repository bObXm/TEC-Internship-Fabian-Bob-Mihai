using Microsoft.EntityFrameworkCore;

namespace Internship.Model
{
    public class APIDbContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<PersonDetail> PersonDetails { get; set; }
        public string DbPath { get; }

        public APIDbContext()
        {
            var path = "C:\\Users\\mihai\\Desktop\\TEC-Internship\\Database";
            DbPath = System.IO.Path.Join(path, "Internship.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

       /* protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonDetail>()
                        .HasOne(pd => pd.Person)
                        .WithMany(p => p.PersonDetails)
                        .HasForeignKey(pd => pd.PersonId);

            modelBuilder.Entity<Person>()
                        .HasOne(p => p.Position)
                        .WithMany(po => po.Persons)
                        .HasForeignKey(p => p.PositionId);

            modelBuilder.Entity<Person>()
                        .HasOne(p => p.Salary)
                        .WithMany(s => s.Persons)
                        .HasForeignKey(p => p.SalaryId);

            modelBuilder.Entity<Position>()
                        .HasOne(po => po.Department)
                        .WithMany(d => d.Positions)
                        .HasForeignKey(po => po.DepartmentId);
        }*/
    }
}
