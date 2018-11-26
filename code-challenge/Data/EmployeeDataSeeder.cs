using challenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private CompensationContext _compensationContext;

        private const String EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext, CompensationContext compensationContext)
        {
            _employeeContext = employeeContext;
            _compensationContext = compensationContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Employees.Any())
            {
                List<Employee> employees = LoadEmployees();
                _employeeContext.Employees.AddRange(employees);

                await _employeeContext.SaveChangesAsync();

                List<Compensation> compensations = CreateDefaultCompensation(employees);
                _compensationContext.Compensations.AddRange(compensations);

                await _compensationContext.SaveChangesAsync();


            }
        }

        private List<Employee> LoadEmployees()
        {
            using (FileStream fs = new FileStream(EMPLOYEE_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Employee> employees = serializer.Deserialize<List<Employee>>(jr);
                FixUpReferences(employees);                

                return employees;
            }
        }

        private void FixUpReferences(List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                select new { Id = employee.EmployeeId, EmployeeRef = employee };

            employees.ForEach(employee =>
            {
                
                if (employee.DirectReports != null)
                {
                    var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                    employee.DirectReports.ForEach(report =>
                    {
                        var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                        referencedEmployees.Add(referencedEmployee);
                    });
                    employee.DirectReports = referencedEmployees;
                }
            });
        }

        // KFD
        // TODO If we want true seeded data, then update the values in the JSON seed file and process them here
        // Opted to default to no salaray and the earliest date.
        private List<Compensation> CreateDefaultCompensation(List<Employee> employees)
        {
            var compensations = new List<Compensation>(employees.Count);
            employees.ForEach(employee =>
            {
                var compensation = new Compensation
                {
                    EmployeeId = employee.EmployeeId,
                    EffectiveDate = new DateTime(0),
                    Salary = 0
                };
                compensations.Add(compensation);
            });
            return compensations;
        }
    }
}
