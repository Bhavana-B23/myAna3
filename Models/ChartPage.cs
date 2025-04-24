using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataVizNavigator1.Models
{
    public class ChartPage
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<PageChartMapping> PageChartMappings { get; set; }
    }
}