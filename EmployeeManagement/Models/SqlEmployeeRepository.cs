using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<SqlEmployeeRepository> _logger;
        private readonly AppDbContext _context;

        public SqlEmployeeRepository(AppDbContext context, ILogger<SqlEmployeeRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public Employee Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _context.Employees.ToList();
        }

        public Employee GetEmployee(int id)
        {
            _logger.LogTrace("Trace log");
            _logger.LogDebug("Debug log");

            return _context.Employees.Find(id);
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = _context.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return employeeChanges;
        }
    }
}