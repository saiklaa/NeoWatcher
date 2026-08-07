    using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoWatcher.Models;
using NeoWatcher.Dto;
using NeoWatcher.Services;

namespace NeoWatcher.Controllers;

public class NeoController : Controller
{
    private readonly NeoStatsCalculator _calculator;

    public NeoController(NeoStatsCalculator calculator) => _calculator = calculator;

    public async Task<IActionResult> Index([FromQuery] NeoFilterViewModel filter)
    {
        var grouped = await _calculator.GetStatsForViewAsync(filter ?? new NeoFilterViewModel());

        // apply simple sort via shared sorter for ViewModel
        grouped = NeoStatsSorter.ApplySort(grouped, filter?.SortBy, filter?.SortDir);

        ViewBag.Filter = filter ?? new NeoFilterViewModel();
        return View(grouped);
    }
}
