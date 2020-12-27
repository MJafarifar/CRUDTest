using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CRUDTest
{
    public class TestState
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class Parent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Child> Children { get; set; }
    }

    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Data Source=.;Initial Catalog=CourseDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TestState>().Property<bool>("isDeleted");
            builder.Entity<TestState>().HasQueryFilter(m => EF.Property<bool>(m, "isDeleted") == false);
        }
        public DbSet<TestState> TestStates { get; set; }
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["isDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["isDeleted"] = true;
                        break;
                }
            }
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            CheckState();
            MyDbContext ctx = new MyDbContext();
            var state=ctx.Find<TestState>(2);
            ctx.Remove(state);
            ctx.SaveChanges();
           
        }

        private static void CheckState()
        {
            TestState testState = new TestState
            {
                Name = "نام دوم"
            };
            MyDbContext ctx = new MyDbContext();
            Console.WriteLine(ctx.Entry(testState).State);
            ctx.Add(testState);
            Console.WriteLine(ctx.Entry(testState).State);
            ctx.SaveChanges();
            Console.WriteLine(ctx.Entry(testState).State);
        }
    }
}
