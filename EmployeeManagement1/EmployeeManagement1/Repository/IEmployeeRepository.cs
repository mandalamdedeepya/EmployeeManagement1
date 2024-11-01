using EmployeeManagement.Models;

namespace EmployeeManagement.Repository
{
    public interface IEmployeeRepository
    {
        Task<PaginatedResult<Employee>> GetAllAsync(string filter, string sort, int page, int pageSize);
        Task<Employee> GetByIdAsync(int id);
        Task<PaginatedResult<Employee>> GetByIdsAsync(List<int> ids, string filter, string sort, int page, int pageSize);
        Task<bool> IsExist(int id);
        Task<bool> IsEmployeeExists(string email);
        Task<int> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee, Employee existingEmployee);
        Task DeleteAsync(int id);
    }
}