using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        // KFD
        // TODO As originially written, this seems to drop DirectReports in the return response. Not sure why? Is this an unwritten task in the challenge?
        public Employee GetById(string id)
        {
            //var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            //return employee;
            var employee = _employeeContext.Employees.AsEnumerable().Where(e => e.EmployeeId == id);
            return employee.SingleOrDefault(); 
        }

        // KFD 
        // Get all Employee records at once
        // TODO This works fine for this small sample, but would need to be updated to paginate results for large data sets
        public List<Employee> GetAll()
        {
            return _employeeContext.Employees.ToList();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
