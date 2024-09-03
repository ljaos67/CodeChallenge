using CodeChallenge.Models;
using System;
using System.Threading.Tasks;
//<summary>
// Interface for Repository layer of the Employee data type. Provides declarations for the Add, retrieve, and remove employee functions.
//<summary>
namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
    }
}