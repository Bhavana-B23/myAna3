using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataVizNavigator1.Services
{
    // Services/DataProcessingService.cs
    public interface IDataProcessingService
    {
        Task<IEnumerable<string>> GetColumnsFromMappingAsync(int mappingId);
        Task<string> ProcessChartDataAsync(Mapping mapping, string xColumn, string yColumn, string chartType);
        Task<List<Dictionary<string, object>>> ExecuteSqlQueryAsync(string connectionString, string sqlQuery);
    }

    public class DataProcessingService : IDataProcessingService
    {
        private readonly ApplicationDbContext _context;

        public DataProcessingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> GetColumnsFromMappingAsync(int mappingId)
        {
            var mapping = await _context.Mappings.FindAsync(mappingId);
            if (mapping == null)
            {
                return Enumerable.Empty<string>();
            }

            if (mapping.DataSourceType == "SQL" && !string.IsNullOrEmpty(mapping.ConnectionString) && !string.IsNullOrEmpty(mapping.SqlQuery))
            {
                try
                {
                    var data = await ExecuteSqlQueryAsync(mapping.ConnectionString, mapping.SqlQuery);
                    if (data.Any())
                    {
                        return data.First().Keys;
                    }
                }
                catch (Exception)
                {
                    return Enumerable.Empty<string>();
                }
            }
            else if (mapping.DataSourceType == "JSON" && !string.IsNullOrEmpty(mapping.TableData))
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(mapping.TableData);
                    if (data?.Any() == true)
                    {
                        return data.First().Keys;
                    }
                }
                catch (Exception)
                {
                    return Enumerable.Empty<string>();
                }
            }

            return Enumerable.Empty<string>();
        }

        public async Task<string> ProcessChartDataAsync(Mapping mapping, string xColumn, string yColumn, string chartType)
        {
            if (mapping.DataSourceType == "SQL")
            {
                var data = await ExecuteSqlQueryAsync(mapping.ConnectionString, mapping.SqlQuery);
                return GenerateChartData(data, xColumn, yColumn, chartType);
            }
            else
            {
                var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(mapping.TableData);
                return GenerateChartData(data, xColumn, yColumn, chartType);
            }
        }

        private string GenerateChartData(List<Dictionary<string, object>> data, string xColumn, string yColumn, string chartType)
        {
            if (data == null || !data.Any())
            {
                return "{}";
            }

            var labels = data.Select(row => row.ContainsKey(xColumn) ? row[xColumn]?.ToString() : null)
                             .Where(x => x != null)
                             .ToArray();

            var dataPoints = data.Select(row => {
                if (row.ContainsKey(yColumn) && row[yColumn] != null)
                {
                    try
                    {
                        return Convert.ToDouble(row[yColumn]);
                    }
                    catch
                    {
                        return 0.0;
                    }
                }
                return 0.0;
            }).ToArray();

            // Different colors for different chart types
            string backgroundColor = chartType == "pie" ?
                "[" + string.Join(",", GenerateRandomColors(labels.Length)) + "]" :
                "'rgba(75, 192, 192, 0.2)'";

            string borderColor = chartType == "pie" ?
                "[" + string.Join(",", GenerateRandomColors(labels.Length, 1)) + "]" :
                "'rgba(75, 192, 192, 1)'";

            var chartData = new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        label = yColumn,
                        data = dataPoints,
                        backgroundColor = backgroundColor,
                        borderColor = borderColor,
                        borderWidth = 1
                    }
                }
            };

            return JsonConvert.SerializeObject(chartData);
        }

        private List<string> GenerateRandomColors(int count, double alpha = 0.2)
        {
            var colors = new List<string>();
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                int r = random.Next(0, 256);
                int g = random.Next(0, 256);
                int b = random.Next(0, 256);
                colors.Add($"'rgba({r}, {g}, {b}, {alpha})'");
            }

            return colors;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteSqlQueryAsync(string connectionString, string sqlQuery)
        {
            var result = new List<Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }
    }
}
