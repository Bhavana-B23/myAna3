using System.ComponentModel.DataAnnotations;

namespace DataVizNavigator1.Models
{
    public class PageChartMapping
    {
        public int Id { get; set; }

        [Required]
        public int PageId { get; set; }

        [Required]
        public int ChartId { get; set; }

        public int DisplayOrder { get; set; }

        // Navigation properties
        public ChartPage ChartPage { get; set; }
        public Chart Chart { get; set; }
    }
}
