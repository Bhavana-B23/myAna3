using DataVizNavigator1.Models;
using System.Collections.Generic;

namespace DataVizNavigator1.Models
{
    public class DashboardViewModel
    {
        public int TotalMappings { get; set; }
        public int TotalCharts { get; set; }
        public List<Chart> RecentCharts { get; set; } = new List<Chart>();
        public string MostUsedChartType { get; set; } = "bar";

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        // Helper properties for pagination UI
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}