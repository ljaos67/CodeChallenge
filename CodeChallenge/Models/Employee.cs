using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//<summary>
//A datatype to represent employee details as well as a List of employees that report to them.
//<summary>
namespace CodeChallenge.Models
{
    public class Employee
    {
        public String EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        public List<Employee> DirectReports { get; set; }
    }
}
