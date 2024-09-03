using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//<summary>
// Interface for Service layer for the Employee data type. Validates employee data before calling repository functions to add to database. 
// Function declarations for adding, getting, or updating employee data.
// Function declarations for retrieving and calculating employee reporting structure.
// Function declarations for creating and retrieving employee compensation data. 
//<summary>;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetReportingStructure(string employeeId);
        void CreateCompensation(Compensation compensation);
        Compensation GetCompensationByEmployeeId(string employeeId);
    }
}
