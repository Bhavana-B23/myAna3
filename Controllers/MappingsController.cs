using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataVizNavigator1.Controllers
{
    public class MappingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MappingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mappings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mappings.ToListAsync());
        }

        public IActionResult Create()
        {
            // Initialize with empty values to avoid null reference exceptions
            var model = new MappingViewModel
            {
                Name = string.Empty,
                Description = string.Empty,
                ConnectionString = string.Empty,
                SqlQuery = string.Empty,
                TableData = string.Empty,
                DataSourceType = "SQL" // Set a default value for the required property
            };
            return View(model);
        }

        // POST: Mappings/Create with validation for SQL and JSON data source
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MappingViewModel model)
        {
            // Ensure properties aren't null
            model.Description ??= string.Empty;
            model.ConnectionString ??= string.Empty;
            model.SqlQuery ??= string.Empty;
            model.TableData ??= string.Empty;

            // Clear ModelState errors for fields that aren't required based on data source type
            if (model.DataSourceType == "SQL" && ModelState.ContainsKey("TableData"))
            {
                ModelState["TableData"].Errors.Clear();

                // Execute the SQL query and store the result as JSON if there's a connection string and SQL query
                if (!string.IsNullOrWhiteSpace(model.ConnectionString) && !string.IsNullOrWhiteSpace(model.SqlQuery))
                {
                    try
                    {
                        model.TableData = await ExecuteSqlQueryToJson(model.ConnectionString, model.SqlQuery);
                    }
                    catch (Exception ex)
                    {
                        // If there's an error, don't stop the form submission but set empty JSON
                        model.TableData = "[]";
                    }
                }
                else
                {
                    model.TableData = "[]"; // Default empty JSON array
                }
            }
            else if (model.DataSourceType == "JSON")
            {
                if (ModelState.ContainsKey("ConnectionString"))
                    ModelState["ConnectionString"].Errors.Clear();

                if (ModelState.ContainsKey("SqlQuery"))
                    ModelState["SqlQuery"].Errors.Clear();
            }

            // Custom validation based on data source type
            if (model.DataSourceType == "SQL")
            {
                if (string.IsNullOrWhiteSpace(model.ConnectionString))
                {
                    ModelState.AddModelError("ConnectionString", "Connection string is required for SQL data source.");
                }

                if (string.IsNullOrWhiteSpace(model.SqlQuery))
                {
                    ModelState.AddModelError("SqlQuery", "SQL query is required for SQL data source.");
                }
            }
            else if (model.DataSourceType == "JSON")
            {
                if (string.IsNullOrWhiteSpace(model.TableData))
                {
                    ModelState.AddModelError("TableData", "Table data is required for JSON data source.");
                }
                else
                {
                    try
                    {
                        // Validate that the input is valid JSON
                        JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(model.TableData);
                    }
                    catch
                    {
                        ModelState.AddModelError("TableData", "Invalid JSON format. Please provide valid JSON data.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapping = new Mapping
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ConnectionString = model.ConnectionString,
                        SqlQuery = model.SqlQuery,
                        DataSourceType = model.DataSourceType,
                        TableData = model.TableData,
                        CreatedOn = DateTime.Now
                    };

                    _context.Mappings.Add(mapping);
                    await _context.SaveChangesAsync();

                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Mapping has been saved successfully!";

                    return RedirectToAction(nameof(Index)); // Redirect to the View Mappings page after saving
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError(string.Empty, $"Error saving mapping: {ex.Message}");
                }
            }

            return View(model); // If the model is invalid, return to the Create page
        }

        // GET: Mappings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mapping = await _context.Mappings.FindAsync(id);
            if (mapping == null) return NotFound();

            var viewModel = new MappingViewModel
            {
                Id = mapping.Id,
                Name = mapping.Name,
                Description = mapping.Description,
                ConnectionString = mapping.ConnectionString,
                SqlQuery = mapping.SqlQuery,
                DataSourceType = mapping.DataSourceType,
                TableData = mapping.TableData
            };

            return View(viewModel);
        }

        // POST: Mappings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MappingViewModel model)
        {
            if (id != model.Id) return NotFound();

            // Ensure properties aren't null
            model.Description ??= string.Empty;
            model.ConnectionString ??= string.Empty;
            model.SqlQuery ??= string.Empty;
            model.TableData ??= string.Empty;

            // Clear ModelState errors for fields that aren't required based on data source type
            if (model.DataSourceType == "SQL" && ModelState.ContainsKey("TableData"))
            {
                ModelState["TableData"].Errors.Clear();

                // Execute the SQL query and store the result as JSON if there's a connection string and SQL query
                if (!string.IsNullOrWhiteSpace(model.ConnectionString) && !string.IsNullOrWhiteSpace(model.SqlQuery))
                {
                    try
                    {
                        model.TableData = await ExecuteSqlQueryToJson(model.ConnectionString, model.SqlQuery);
                    }
                    catch (Exception ex)
                    {
                        // If there's an error, don't stop the form submission but set empty JSON
                        model.TableData = "[]";
                    }
                }
                else
                {
                    model.TableData = "[]"; // Default empty JSON array
                }
            }
            else if (model.DataSourceType == "JSON")
            {
                if (ModelState.ContainsKey("ConnectionString"))
                    ModelState["ConnectionString"].Errors.Clear();

                if (ModelState.ContainsKey("SqlQuery"))
                    ModelState["SqlQuery"].Errors.Clear();
            }

            // Custom validation based on data source type
            if (model.DataSourceType == "SQL")
            {
                if (string.IsNullOrWhiteSpace(model.ConnectionString))
                {
                    ModelState.AddModelError("ConnectionString", "Connection string is required for SQL data source.");
                }

                if (string.IsNullOrWhiteSpace(model.SqlQuery))
                {
                    ModelState.AddModelError("SqlQuery", "SQL query is required for SQL data source.");
                }
            }
            else if (model.DataSourceType == "JSON")
            {
                if (string.IsNullOrWhiteSpace(model.TableData))
                {
                    ModelState.AddModelError("TableData", "Table data is required for JSON data source.");
                }
                else
                {
                    try
                    {
                        // Validate that the input is valid JSON
                        JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(model.TableData);
                    }
                    catch
                    {
                        ModelState.AddModelError("TableData", "Invalid JSON format. Please provide valid JSON data.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapping = await _context.Mappings.FindAsync(id);
                    if (mapping == null) return NotFound();

                    mapping.Name = model.Name;
                    mapping.Description = model.Description;
                    mapping.ConnectionString = model.ConnectionString;
                    mapping.SqlQuery = model.SqlQuery;
                    mapping.DataSourceType = model.DataSourceType;
                    mapping.TableData = model.TableData;

                    _context.Update(mapping);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Mapping has been updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MappingExists(id)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating mapping: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: Mappings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mapping = await _context.Mappings.FirstOrDefaultAsync(m => m.Id == id);
            if (mapping == null) return NotFound();

            return View(mapping);
        }

        // GET: Mappings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mapping = await _context.Mappings.FirstOrDefaultAsync(m => m.Id == id);
            if (mapping == null) return NotFound();

            return View(mapping);
        }

        // POST: Mappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mapping = await _context.Mappings.FindAsync(id);
            if (mapping != null)
            {
                _context.Mappings.Remove(mapping);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Mapping has been deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MappingExists(int id)
        {
            return _context.Mappings.Any(e => e.Id == id);
        }

        // POST: Mappings/TestResult
        [HttpPost]
        public async Task<IActionResult> TestResult(string connectionString, string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(sqlQuery))
            {
                return Json(new { success = false, message = "Connection string and SQL query are required." });
            }

            try
            {
                // Establish the connection and execute the SQL query
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(sqlQuery, connection);
                    var reader = await command.ExecuteReaderAsync();

                    var result = new List<Dictionary<string, object>>();

                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i] is DBNull ? null : reader[i];
                        }
                        result.Add(row);
                    }

                    // Convert the result to JSON string
                    string jsonData = JsonConvert.SerializeObject(result);

                    return Json(new { success = true, data = result, jsonData = jsonData });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Mappings/SaveTestData
        [HttpPost]
        public async Task<IActionResult> SaveTestData(int id, string jsonData)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(jsonData))
            {
                return Json(new { success = false, message = "Invalid ID or JSON data" });
            }

            try
            {
                var mapping = await _context.Mappings.FindAsync(id);
                if (mapping == null)
                {
                    return Json(new { success = false, message = "Mapping not found" });
                }

                // Validate JSON format
                try
                {
                    JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Invalid JSON format" });
                }

                // Update the TableData field
                mapping.TableData = jsonData;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Table data saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper method to execute SQL query and return JSON
        private async Task<string> ExecuteSqlQueryToJson(string connectionString, string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(sqlQuery, connection);
                var reader = await command.ExecuteReaderAsync();

                var result = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader[i] is DBNull ? null : reader[i];
                    }
                    result.Add(row);
                }

                return JsonConvert.SerializeObject(result);
            }
        }
    }
}