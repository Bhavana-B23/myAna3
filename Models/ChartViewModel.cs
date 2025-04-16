namespace DataVizNavigator1.Models;
using System.ComponentModel.DataAnnotations;
public class ChartViewModel
{
    public int Id { get; set; }

    [Display(Name = "Chart Name")]
    public string? Name { get; set; } // Made optional by removing [Required] and making nullable

    [Required]
    [Display(Name = "Dataset")]
    public int MappingId { get; set; }

    [Required]
    [Display(Name = "Chart Type")]
    public string ChartType { get; set; }

    [Required]
    [Display(Name = "X-Axis Column")]
    public string XAxisColumn { get; set; }

    [Required]
    [Display(Name = "Y-Axis Column")]
    public string YAxisColumn { get; set; }

    public IEnumerable<Mapping>? AvailableMappings { get; set; } = Enumerable.Empty<Mapping>();
    public IEnumerable<string>? AvailableColumns { get; set; } = Enumerable.Empty<string>();
}