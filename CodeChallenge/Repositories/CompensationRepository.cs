using System;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
//<summary>
// Repository layer for Compensation data type. Calls employee database context to add compensation data or query compensation data
//<summary>
namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;
        
        // summary: Initializes employeeContext object for database use
        // inputs: employeeContext - EmployeeContext object
        // returns: none
        public CompensationRepository(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        // summary: Adds compensation object to database
        // inputs: compensation -- Compensation object to add
        // returns: compensation -- Returns compensation object
        public Compensation Add(Compensation compensation)
        {
            // Check if the Employee is already tracked by the context
            var trackedEmployee = _employeeContext.Employees
                .SingleOrDefault(e => e.EmployeeId == compensation.Employee.EmployeeId);

            if (trackedEmployee != null)
            {
                // If the employee is already tracked, use that instance
                compensation.Employee = trackedEmployee;
            }

            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }
        // summary: Validates Employee object, sends object to repository layer
        // inputs: employee - Employee object to create
        // returns: employee -- Employee object
        public Compensation GetById(string employeeId)
        {
            return _employeeContext.Compensations
                .Include(c => c.Employee)
                .SingleOrDefault(c => c.Employee.EmployeeId == employeeId);
        }
        // summary: saves employee compensation data changes in database
        // inputs: none
        // returns: _employeeContext saved changes task
        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
