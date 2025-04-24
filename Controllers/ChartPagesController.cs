using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataVizNavigator1.Controllers
{
    public class ChartPagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartPagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChartPages
        public async Task<IActionResult> Index()
        {
            var chartPages = await _context.ChartPages
                .Include(p => p.PageChartMappings)
                    .ThenInclude(pcm => pcm.Chart)
                .ToListAsync();

            return View(chartPages);
        }

        // GET: ChartPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartPage = await _context.ChartPages
                .Include(p => p.PageChartMappings)
                    .ThenInclude(pcm => pcm.Chart)
                        .ThenInclude(c => c.Mapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chartPage == null)
            {
                return NotFound();
            }

            return View(chartPage);
        }

        // GET: ChartPages/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ChartPageViewModel
            {
                AvailableCharts = await _context.Charts
                    .Include(c => c.Mapping)
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: ChartPages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChartPageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Create the chart page
                var chartPage = new ChartPage
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    CreatedOn = DateTime.Now
                };

                _context.Add(chartPage);
                await _context.SaveChangesAsync();

                // Add chart mappings
                if (viewModel.ChartSelections != null && viewModel.ChartSelections.Any())
                {
                    foreach (var chartSelection in viewModel.ChartSelections.Where(cs => cs.ChartId > 0))
                    {
                        var pageChartMapping = new PageChartMapping
                        {
                            PageId = chartPage.Id,
                            ChartId = chartSelection.ChartId,
                            DisplayOrder = chartSelection.DisplayOrder
                        };

                        _context.PageChartMappings.Add(pageChartMapping);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; reload available charts
            viewModel.AvailableCharts = await _context.Charts
                .Include(c => c.Mapping)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: ChartPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartPage = await _context.ChartPages
                .Include(p => p.PageChartMappings)
                    .ThenInclude(pcm => pcm.Chart)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chartPage == null)
            {
                return NotFound();
            }

            var viewModel = new ChartPageViewModel
            {
                Id = chartPage.Id,
                Name = chartPage.Name,
                Description = chartPage.Description,
                CreatedOn = chartPage.CreatedOn,
                AvailableCharts = await _context.Charts
                    .Include(c => c.Mapping)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                ChartSelections = chartPage.PageChartMappings
                    .Select(pcm => new ChartSelectionViewModel
                    {
                        ChartId = pcm.ChartId,
                        ChartName = pcm.Chart.Name,
                        ChartType = pcm.Chart.ChartType,
                        DisplayOrder = pcm.DisplayOrder
                    }).ToList()
            };

            return View(viewModel);
        }

        // POST: ChartPages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChartPageViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update chart page properties
                    var chartPage = await _context.ChartPages.FindAsync(id);
                    if (chartPage == null)
                    {
                        return NotFound();
                    }

                    chartPage.Name = viewModel.Name;
                    chartPage.Description = viewModel.Description;

                    _context.Update(chartPage);

                    // Remove existing chart mappings
                    var existingMappings = await _context.PageChartMappings
                        .Where(pcm => pcm.PageId == id)
                        .ToListAsync();

                    _context.PageChartMappings.RemoveRange(existingMappings);

                    // Add updated chart mappings
                    if (viewModel.ChartSelections != null && viewModel.ChartSelections.Any())
                    {
                        foreach (var chartSelection in viewModel.ChartSelections.Where(cs => cs.ChartId > 0))
                        {
                            var pageChartMapping = new PageChartMapping
                            {
                                PageId = chartPage.Id,
                                ChartId = chartSelection.ChartId,
                                DisplayOrder = chartSelection.DisplayOrder
                            };

                            _context.PageChartMappings.Add(pageChartMapping);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartPageExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If we got this far, something failed; reload available charts
            viewModel.AvailableCharts = await _context.Charts
                .Include(c => c.Mapping)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: ChartPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartPage = await _context.ChartPages
                .Include(p => p.PageChartMappings)
                    .ThenInclude(pcm => pcm.Chart)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chartPage == null)
            {
                return NotFound();
            }

            return View(chartPage);
        }

        // POST: ChartPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chartPage = await _context.ChartPages.FindAsync(id);
            if (chartPage == null)
            {
                return NotFound();
            }

            // Delete associated mappings first
            var pageChartMappings = await _context.PageChartMappings
                .Where(pcm => pcm.PageId == id)
                .ToListAsync();

            _context.PageChartMappings.RemoveRange(pageChartMappings);

            // Then delete the chart page
            _context.ChartPages.Remove(chartPage);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ChartPages/View/5 (public view for users to see the charts)
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartPage = await _context.ChartPages
                .Include(p => p.PageChartMappings)
                    .ThenInclude(pcm => pcm.Chart)
                        .ThenInclude(c => c.Mapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chartPage == null)
            {
                return NotFound();
            }

            // Order charts by display order
            chartPage.PageChartMappings = chartPage.PageChartMappings
                .OrderBy(pcm => pcm.DisplayOrder)
                .ToList();

            return View(chartPage);
        }

        private bool ChartPageExists(int id)
        {
            return _context.ChartPages.Any(e => e.Id == id);
        }

        // API endpoint to get chart details
        [HttpGet]
        public async Task<IActionResult> GetChartDetails(int chartId)
        {
            var chart = await _context.Charts
                .Include(c => c.Mapping)
                .FirstOrDefaultAsync(c => c.Id == chartId);

            if (chart == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = chart.Id,
                name = chart.Name,
                chartType = chart.ChartType,
                mappingName = chart.Mapping?.Name
            });
        }
    }
}