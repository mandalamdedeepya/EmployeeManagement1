namespace EmployeeManagement.Models
{
    public class PaginatedResult<T>
    {
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public required List<T> Items { get; set; }
    }
}
