using System.ComponentModel.DataAnnotations;

namespace DataVizNavigator1.Models
{
    public class MappingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mapping name is required")]
        [Display(Name = "Mapping Name")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        [Display(Name = "SQL Query")]
        public string SqlQuery { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a data source type")]
        [Display(Name = "Data Source Type")]
        public required string DataSourceType { get; set; }

        // Remove required attribute from TableData
        [Display(Name = "Table Data")]
        public string TableData { get; set; } = string.Empty;
    }
}