using System;

namespace NeoWatcher.Models;

public class NeoStatViewModel
{
    public DateTime Date { get; set; }
    public int ObjectCount { get; set; }
    public double MaxDiameter { get; set; }
    public double AvgVelocity { get; set; }
    public bool HasHazardousObjects { get; set; }
}
