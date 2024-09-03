using System;

//<summary>
//A datatype to represent an employees compensation details
//<summary>
namespace CodeChallenge.Models
{
    public class Compensation
    {
        //Generates a unique Guid
        public Compensation()
        {
            CompensationId = Guid.NewGuid().ToString();
        }
        public string CompensationId { get; set; }
        public Employee Employee { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
