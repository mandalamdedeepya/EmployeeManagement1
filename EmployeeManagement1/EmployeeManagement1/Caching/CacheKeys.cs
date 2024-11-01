namespace EmployeeManagement.Caching
{
    public static class CacheKeys
    {
        public static string AllEmployees = "AllEmployees";
        public static string EmployeeById(int id) => $"Employee:{id}";
        public static string EmployeesByIds(string ids, string filter, string sort, int page, int pageSize)
            => $"EmployeesByIds:{ids}:Filter:{filter}:Sort:{sort}:Page:{page}:PageSize:{pageSize}";
    }
}

