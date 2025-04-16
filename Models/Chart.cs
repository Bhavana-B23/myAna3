namespace DataVizNavigator1.Models;

using System.ComponentModel.DataAnnotations;

public class Chart
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public int MappingId { get; set; }
    public Mapping? Mapping { get; set; }

    [Required]
    public required string ChartType { get; set; }

    [Required]
    public required string XAxisColumn { get; set; }

    [Required]
    public required string YAxisColumn { get; set; }

    [Required]
    public required string ChartData { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}
