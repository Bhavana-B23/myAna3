using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataVizNavigator1.Models
{
    public class ChartPageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Page name is required")]
        [Display(Name = "Page Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // For chart selection
        public List<int> SelectedChartIds { get; set; } = new List<int>();

        // Available charts for selection
        public IEnumerable<Chart> AvailableCharts { get; set; } = new List<Chart>();

        // Selected charts with display order
        public List<ChartSelectionViewModel> ChartSelections { get; set; } = new List<ChartSelectionViewModel>();
    }

    public class ChartSelectionViewModel
    {
        public int ChartId { get; set; }
        public string ChartName { get; set; }
        public string ChartType { get; set; }
        public int DisplayOrder { get; set; }
    }
}