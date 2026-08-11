using Microsoft.AspNetCore.Mvc;
using NeoWatcher.Models;
using NeoWatcher.Services;

namespace NeoWatcher.Controllers;

public class NeoController : Controller
{
    private readonly NeoStatsCalculator _calculator;

    public NeoController(NeoStatsCalculator calculator) => _calculator = calculator;

    public async Task<IActionResult> Index([FromQuery] NeoFilterViewModel filter)
    {
        var grouped = await _calculator.GetStatsForViewAsync(filter ?? new NeoFilterViewModel());

        ViewBag.Filter = filter ?? new NeoFilterViewModel();
        return View(grouped);
    }
}
