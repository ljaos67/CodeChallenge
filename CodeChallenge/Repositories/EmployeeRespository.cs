using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
//<summary>
// Repository layer for the Employee data type. Adds, retrieves, or removes employee by calling associated function from database context.
//<summary>
namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<IEmployeeRepository> _logger;

        // summary: Employee repository constructor
        // inputs: logger -- employeeRepository interface logging object, employeeContext -- EmployeeContext object
        // returns: none
        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        // summary: Creates new employee, saves to database
        // inputs: employee - Employee object to create
        // returns: employee -- Employee object
        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        // summary: Queries database by employee id
        // inputs: id - Employee id to query
        // returns: employee object from database
        public Employee GetById(string id)
        {
            // Eager loads DirectReports, otherwise would return null
            return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
        }

        // summary: saves employee data changes in database
        // inputs: none
        // returns: _employeeContext saved changes task
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
        // summary: Removes employee
        // inputs: employee - Employee object to remove
        // returns: employee -- Employee object removed
        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
