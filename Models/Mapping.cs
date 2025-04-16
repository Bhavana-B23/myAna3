using System;

namespace DataVizNavigator1.Models
{
    public class Mapping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string SqlQuery { get; set; } = string.Empty;
        public string DataSourceType { get; set; }
        public string TableData { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}