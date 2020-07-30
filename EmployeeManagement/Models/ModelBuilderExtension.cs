using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee() { Id = 1, Name = "Mary", Department = Dept.HR, Email = "mary@work.com" },
                new Employee() { Id = 2, Name = "John", Department = Dept.IT, Email = "john@work.com" },
                new Employee() { Id = 3, Name = "Sam", Department = Dept.IT, Email = "sam@work.com" }
                );
        }
    }
}