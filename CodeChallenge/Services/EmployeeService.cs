using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
//<summary>
// Service layer for the Employee data type. Validates employee data before calling repository functions to add to database. 
// Function definitions for adding, getting, or updating employee data.
// Function definitions for retrieving and calculating employee reporting structure.
// Function definitions for creating and retrieving employee compensation data. 
//<summary>
namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ICompensationRepository _compensationRepository;

        // summary: Employee service constructor, initializes Repository objects 
        // inputs: employeeRepository -- employee repository layer object, compensationRepository -- compensation repository layer object
        // returns: none
        public EmployeeService(IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
        }

        // summary: Validates Employee object, sends object to repository layer
        // inputs: employee - Employee object to create
        // returns: employee -- Employee object
        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }
        // summary: Validates EmployeeId, sends object to repository layer to query
        // inputs: EmployeeId -- Employee's Id number
        // returns: Employee object from database or null if not found
        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }
        // summary: Validates originalEmployee object, newEmployee object to update employee through repository layer
        // inputs: originalEmployee -- employee information to be updated, newEmployee -- Employee with new replacement data
        // returns: Employee -- Updated employee data
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
        // summary: Queries employeeId, validates employee, calls GetNumberOfReports to calculate employee's reports.
        // inputs: EmployeeId -- the employee's id
        // returns: ReportingStructure -- Employee's Reporting structure
        public ReportingStructure GetReportingStructure(string employeeId)
        {
            var employee = GetById(employeeId);

            if (employee == null)
            {
                return null;
            }

            int numberOfReports = GetNumberOfReports(employee);

            return new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }
        // summary: Recursive function to calculate employee reports
        // inputs: employee- Employee object
        // returns: Integer count employee reports 
        private int GetNumberOfReports(Employee employee)
        {
            if (employee.DirectReports == null)
            {
                return 0;
            }

            int count = employee.DirectReports.Count;
            foreach (var reportId in employee.DirectReports)
            {
                var report = GetById(reportId.EmployeeId);
                count += GetNumberOfReports(report);
            }
            return count;
        }
        // summary: Validates Compensation object, sends to repository layer to add Employee to database
        // inputs: compensation - Compensation object to be added to database
        // returns: none
        public void CreateCompensation(Compensation compensation)
        {
            if (compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
        }
        // summary: Queries compensation object from database by employee id
        // inputs: EmployeeId - employee's id
        // returns: Compensation object from database or null if not found
        public Compensation GetCompensationByEmployeeId(string employeeId)
        {
            return _compensationRepository.GetById(employeeId);
        }
    }
}
