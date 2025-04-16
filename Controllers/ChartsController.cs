using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using DataVizNavigator1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DataVizNavigator1.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProcessingService _dataService;

        public ChartsController(ApplicationDbContext context, IDataProcessingService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: Charts/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new ChartViewModel
            {
                AvailableMappings = await _context.Mappings.ToListAsync()
            };
            return View(viewModel);
        }

        // GET: Charts/GetColumns/5
        public async Task<IActionResult> GetColumns(int id)
        {
            var columns = await _dataService.GetColumnsFromMappingAsync(id);
            return Json(columns);
        }

        // POST: Charts/GenerateChart
        [HttpPost]
        public async Task<IActionResult> GenerateChart(ChartViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mapping = await _context.Mappings.FindAsync(model.MappingId);
                if (mapping == null)
                {
                    return Json(new { success = false, message = "Mapping not found" });
                }

                var chartData = await _dataService.ProcessChartDataAsync(
                    mapping,
                    model.XAxisColumn,
                    model.YAxisColumn,
                    model.ChartType);

                return Json(new { success = true, chartData });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
            return Json(new { success = false, message = "Invalid model", errors });
        }

        // POST: Charts/SaveChart
        [HttpPost]
        public async Task<IActionResult> SaveChart(ChartViewModel model)
        {
            // Validate that Name is provided when saving a chart
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Chart name is required for saving");
            }

            if (ModelState.IsValid)
            {
                var mapping = await _context.Mappings.FindAsync(model.MappingId);
                if (mapping == null)
                {
                    return Json(new { success = false, message = "Mapping not found" });
                }

                var chartData = await _dataService.ProcessChartDataAsync(
                    mapping,
                    model.XAxisColumn,
                    model.YAxisColumn,
                    model.ChartType);

                var chart = new Chart
                {
                    Name = model.Name!,  // Non-null assertion as we've validated it above
                    MappingId = model.MappingId,
                    ChartType = model.ChartType,
                    XAxisColumn = model.XAxisColumn,
                    YAxisColumn = model.YAxisColumn,
                    ChartData = chartData
                };

                _context.Charts.Add(chart);
                await _context.SaveChangesAsync();

                return Json(new { success = true, chartId = chart.Id });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // GET: Charts/Saved
        public async Task<IActionResult> Saved()
        {
            var charts = await _context.Charts
                .Include(c => c.Mapping)
                .ToListAsync();
            return View(charts);
        }

        // GET: Charts/View/5
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
                return NotFound();

            var chart = await _context.Charts
                .Include(c => c.Mapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chart == null)
                return NotFound();

            return View(chart);
        }
    }
}