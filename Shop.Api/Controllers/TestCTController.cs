using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers;

[ApiController]
[Route("test")]
public class TestCTController(ILogger<TestCTController> logger):ControllerBase
{
    [HttpGet("without")]
    public async Task<IActionResult> TestWithoutCT()
    {
        logger.LogInformation("start test withoutCT");
        await Task.Delay(1000);
        logger.LogInformation("action 1");
        await Task.Delay(2000);
        logger.LogInformation("action 2");
        await Task.Delay(500);
        logger.LogInformation("end test withoutCT");

        return Ok();
    }
    [HttpGet("with")]
    public async Task<IActionResult> TestWithCT(CancellationToken cancellationToken)
    {
        logger.LogInformation("start test withCT");
        await Task.Delay(1000, cancellationToken);
        logger.LogInformation("action 1 (with)");
        await Task.Delay(2000, cancellationToken);
        logger.LogInformation("action 2 (with)");
        await Task.Delay(500, cancellationToken);
        logger.LogInformation("end test withCT");
        return Ok();
    }


}
