using System.Collections.Generic;
using System.Linq;
using NeoWatcher.Models;

namespace NeoWatcher.Services;

public static class NeoStatsSorter
{
    public static List<NeoStatViewModel> ApplySort(IEnumerable<NeoStatViewModel> items, string? sortBy, string? sortDir)
    {
        var list = items.ToList();
        var sb = (sortBy ?? "date").ToLowerInvariant();
        var sd = (sortDir ?? "asc").ToLowerInvariant();

        return (sb, sd) switch
        {
            ("count", "desc") => list.OrderByDescending(x => x.ObjectCount).ToList(),
            ("count", _) => list.OrderBy(x => x.ObjectCount).ToList(),
            ("mass", "desc") => list.OrderByDescending(x => x.MaxDiameter).ToList(),
            ("mass", _) => list.OrderBy(x => x.MaxDiameter).ToList(),
            (_, "desc") => list.OrderByDescending(x => x.Date).ToList(),
            _ => list.OrderBy(x => x.Date).ToList(),
        };
    }
}
