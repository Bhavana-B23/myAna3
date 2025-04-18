using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataVizNavigator1.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int DefaultPageSize = 10;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index(int page = 1, int pageSize = DefaultPageSize)
        {
            // Ensure valid pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;

            // Calculate skip for pagination
            int skip = (page - 1) * pageSize;

            // Get total count for pagination
            int totalCharts = await _context.Charts.CountAsync();

            var dashboardViewModel = new DashboardViewModel
            {
                TotalMappings = await _context.Mappings.CountAsync(),
                TotalCharts = totalCharts,
                RecentCharts = await _context.Charts
                    .Include(c => c.Mapping)
                    .OrderByDescending(c => c.CreatedOn)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync(),
                MostUsedChartType = await GetMostUsedChartTypeAsync(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCharts / (double)pageSize)
            };

            return View(dashboardViewModel);
        }

        // API endpoint for loading more charts via AJAX
        [HttpGet]
        public async Task<JsonResult> GetMoreCharts(int page = 1, int pageSize = DefaultPageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;

            int skip = (page - 1) * pageSize;

            var charts = await _context.Charts
                .Include(c => c.Mapping)
                .OrderByDescending(c => c.CreatedOn)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return Json(new
            {
                charts = charts,
                currentPage = page,
                totalPages = (int)Math.Ceiling(await _context.Charts.CountAsync() / (double)pageSize)
            });
        }

        private async Task<string> GetMostUsedChartTypeAsync()
        {
            var chartTypes = await _context.Charts
                .GroupBy(c => c.ChartType)
                .Select(g => new { ChartType = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefaultAsync();
            return chartTypes?.ChartType ?? "bar";
        }
    }
}

