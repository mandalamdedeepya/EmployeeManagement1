using EmployeeManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> RegisterAsync(Login model);
        Task<string> LoginAsync(Login model);
    }
}
