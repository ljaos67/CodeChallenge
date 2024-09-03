//<summary>
//A datatype to calculate the number of employees that report to this employee.
//<summary>
namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        //<summary>
        //Calculates number of reports to this employee
        //<summary>
        //<value>
        // Integer number of reports
        //<value>
        public int NumberOfReports { get; set; }
    }
}
