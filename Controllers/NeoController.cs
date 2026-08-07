using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoWatcher.Models;
using NeoWatcher.Dto;

namespace NeoWatcher.Controllers;

public class NeoController : Controller
{
    private readonly NeoContext _db;

    public NeoController(NeoContext db) => _db = db;

    public async Task<IActionResult> Index([FromQuery] NeoFilterViewModel filter)
    {
        var query = _db.NearEarthObjects.AsQueryable();

        if (filter.From.HasValue)
            query = query.Where(x => x.CloseApproachDate >= filter.From.Value.Date);
        if (filter.To.HasValue)
            query = query.Where(x => x.CloseApproachDate <= filter.To.Value.Date);
        if (filter.Hazardous.HasValue)
            query = query.Where(x => x.IsPotentiallyHazardous == filter.Hazardous.Value);
        if (filter.MinDiameter.HasValue)
            query = query.Where(x => x.EstimatedDiameterMax >= filter.MinDiameter.Value);
        if (filter.MaxDiameter.HasValue)
            query = query.Where(x => x.EstimatedDiameterMin <= filter.MaxDiameter.Value);

        var grouped = await query
            .GroupBy(x => x.CloseApproachDate.Date)
            .Select(g => new NeoStatViewModel
            {
                Date = g.Key,
                ObjectCount = g.Count(),
                MaxDiameter = g.Max(x => x.EstimatedDiameterMax),
                AvgVelocity = g.Average(x => x.RelativeVelocityKmh),
                HasHazardousObjects = g.Any(x => x.IsPotentiallyHazardous)
            })
            .ToListAsync();

        // apply simple sort via shared sorter
        grouped = NeoWatcher.Services.NeoStatsSorter.ApplySort(grouped, filter?.SortBy, filter?.SortDir);

        ViewBag.Filter = filter ?? new NeoFilterViewModel();
        return View(grouped);
    }
}
