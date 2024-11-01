using AutoMapper;
using EmployeeManagement.Caching;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagement.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cacheProvider;

        public EmployeeRepository(EmployeeContext context, IMapper mapper, IMemoryCache cacheProvider)
        {
            _context = context;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public async Task<PaginatedResult<Employee>> GetAllAsync(string filter, string sort, int page, int pageSize)
        {
            if (!_cacheProvider.TryGetValue(CacheKeys.AllEmployees, out PaginatedResult<Employee> employees))
            {
                var query = _context.Employees.AsQueryable();

                //Filtering 
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(employee => employee.Name.Contains(filter) ||
                                             employee.Email.Contains(filter) ||
                                             employee.Location.Contains(filter) ||
                                             employee.Department.Contains(filter));
                }

                // Sorting
                if (!string.IsNullOrWhiteSpace(sort))
                {
                    query = sort.ToLower() switch
                    {
                        "name" => query.OrderBy(employee => employee.Name),
                        "salary" => query.OrderBy(employee => employee.Salary),
                        "createddate" => query.OrderBy(employee => employee.CreatedDate),
                        "location" => query.OrderBy(employee => employee.Location),
                        _ => query.OrderBy(employee => employee.Id),
                    };
                }

                // Pagination
                var totalRecords = await query.CountAsync();
                var employeeList = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                employees = new PaginatedResult<Employee>
                {
                    TotalRecords = totalRecords,
                    Page = page,
                    PageSize = pageSize,
                    Items = _mapper.Map<List<Employee>>(employeeList)
                };

                // Cache result for get all async
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cacheProvider.Set(CacheKeys.AllEmployees, employees, cacheEntryOptions);
            }

            return employees;
        }


        public async Task<Employee> GetByIdAsync(int id)
        {
            if (!_cacheProvider.TryGetValue(CacheKeys.EmployeeById(id), out Employee employee))
            {
                employee = await _context.Employees.FindAsync(id);
                if (employee == null) return null;

                // Cache the employee
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cacheProvider.Set(CacheKeys.EmployeeById(id), employee, cacheEntryOptions);
            }
            return _mapper.Map<Employee>(employee);
        }

        public async Task<PaginatedResult<Employee>> GetByIdsAsync(List<int> ids, string filter, string sort, int page, int pageSize)
        {
            string cacheKey = CacheKeys.EmployeesByIds(string.Join(",", ids), filter, sort, page, pageSize);
            if (!_cacheProvider.TryGetValue(cacheKey, out PaginatedResult<Employee> employees))
            {
                var query = _context.Employees.AsQueryable().Where(e => ids.Contains(e.Id));

                // Filtering
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query = query.Where(e => e.Name.Contains(filter) ||
                                             e.Email.Contains(filter) ||
                                             e.Location.Contains(filter) ||
                                             e.Department.Contains(filter));
                }

                // Sorting
                if (!string.IsNullOrWhiteSpace(sort))
                {
                    query = sort.ToLower() switch
                    {
                        "name" => query.OrderBy(e => e.Name),
                        "salary" => query.OrderBy(e => e.Salary),
                        "createddate" => query.OrderBy(e => e.CreatedDate),
                        "location" => query.OrderBy(e => e.Location),
                        _ => query.OrderBy(e => e.Id),
                    };
                }

                // Pagination
                var totalRecords = await query.CountAsync();
                var employeeList = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                employees = new PaginatedResult<Employee>
                {
                    TotalRecords = totalRecords,
                    Page = page,
                    PageSize = pageSize,
                    Items = _mapper.Map<List<Employee>>(employeeList)
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cacheProvider.Set(cacheKey, employees, cacheEntryOptions);
            }

            return employees;
        }

        public async Task<bool> IsEmployeeExists(string email)
        {
            //var exists = await _context.Employees
            //    .AnyAsync(employee => employee.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            //return exists;
            var lowerCaseEmail = email.ToLower();
            var exists = await _context.Employees
                .AnyAsync(employee => employee.Email.ToLower() == lowerCaseEmail);
            return exists;
        }

        public async Task<bool> IsExist(int id)
        {
            var exists = await _context.Employees.AnyAsync(employee => employee.Id == id);
            return exists;
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            employee.CreatedDate = DateTime.Now;
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _cacheProvider.Remove(CacheKeys.AllEmployees);

            return employee.Id;
        }

        public async Task<Employee> UpdateAsync(Employee employee, Employee existingEmployee)
        {
            _mapper.Map(employee, existingEmployee);
            existingEmployee.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            _cacheProvider.Remove(CacheKeys.AllEmployees);
            _cacheProvider.Remove(CacheKeys.EmployeeById(existingEmployee.Id));

            return existingEmployee;
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                _cacheProvider.Remove(CacheKeys.AllEmployees);
                _cacheProvider.Remove(CacheKeys.EmployeeById(id));
            }
        }
    }
}
