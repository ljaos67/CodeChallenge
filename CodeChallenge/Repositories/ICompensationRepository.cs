using CodeChallenge.Models;
using System.Threading.Tasks;
//<summary>
// Interface for Repository layer for Compensation data type. Calls employee database context to add compensation data or query compensation data.
//<summary>
namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Compensation GetById(string employeeId);
        Task SaveAsync();
    }
}
