using Microsoft.AspNetCore.Mvc;

namespace TicTacToeBank.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase {
	[HttpGet]
	public IActionResult Get() {
		return Ok("Service is running...");
	}
}