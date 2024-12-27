namespace TestTask.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController, Route("[controller]")]
public class ReportController(ReportService marketService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTopNEachYear([FromQuery] int n = 3)
    {
        var reports = await marketService.GenerateReport(n);
        return Ok(reports);
    }
}
