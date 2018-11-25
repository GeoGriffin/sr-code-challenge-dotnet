using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        // KFD
        public List<Employee> GetAllEmployees()
        {
            return _employeeRepository.GetAll();
        }

        // KFD
        public ReportingStructure GetReportingStructureById(string id)
        {
            var employee = GetById(id);
            if (employee != null)
            {
                return new ReportingStructure
                {
                    Employee = employee,
                    NumberOfReports = GetDirectReportCount(employee)
                };
            }

            return null;
        }

        // KFD
        // Recursive method to traverse all DirectReports and tally the total number
        int GetDirectReportCount(Employee employee)
        {
            var reportCount = 0;

            if (employee.DirectReports != null)
            {
                _logger.LogDebug($"'{employee.FirstName} {employee.LastName}' has '{employee.DirectReports.Count}' direct reports");
                foreach (Employee directReport in employee.DirectReports)
                {
                    // Add one for this direct report
                    reportCount++;

                    // Recursively add the direct report's direct reports.
                    reportCount += GetDirectReportCount(directReport);
                }
            }
            else
            {
                _logger.LogDebug($"'{employee.FirstName} {employee.LastName}' has NO direct reports");
            }

            return reportCount;
        }
    }
}
