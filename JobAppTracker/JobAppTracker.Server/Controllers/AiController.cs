using JobAppTracker.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobAppTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly AIService _aiService;

        public AIController(AIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("analyse")]
        public async Task<IActionResult> AnalyseJobDescription([FromBody] JobDescriptionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.JobDescription))
                return BadRequest("Job description cannot be empty");

            var result = await _aiService.AnalyseJobDescription(request.JobDescription);
            return Ok(result);
        }
    }

    public class JobDescriptionRequest
    {
        public string JobDescription { get; set; }
    }
}