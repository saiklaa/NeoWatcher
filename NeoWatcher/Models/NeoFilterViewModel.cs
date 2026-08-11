using System;

namespace NeoWatcher.Models;

public class NeoFilterViewModel
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool? Hazardous { get; set; }
    public double? MinDiameter { get; set; }
    public double? MaxDiameter { get; set; }
    public string SortBy { get; set; } = "date";
    public string SortDir { get; set; } = "asc";
}
