using Microsoft.AspNetCore.Mvc;
using DirWatcher.Models;
using DirWatcher.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DirWatcher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirWatcherController : ControllerBase
    {
        private readonly DirWatcherService _dirWatcherService;
        private readonly ILogger<DirWatcherController> _logger;
       
        public DirWatcherController(DirWatcherService dirWatcherService, ILogger<DirWatcherController> logger)
        {
            
            _dirWatcherService = dirWatcherService;
            _logger = logger;
        }
        
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            bool isRunning = _dirWatcherService.IsTaskRunning();
            string status = isRunning ? "Running" : "Stopped";
            return Ok($"Background task status: {status}");
        }

        [HttpPost("StartBackgroundTask")]
        public IActionResult StartBackgroundTask([FromBody] WatchRequest request)
        {
            try
            {
                _dirWatcherService.StartBackgroundTask(request);
                return Ok("Background task started.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("stopBackgroundTask")]
        public async Task<IActionResult> StopBackgroundTask()
        {
            try
            {
                _logger.LogInformation("Stopping background task...");
                await _dirWatcherService.StopWatching();
                return Ok("Background task stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while stopping background task");
                return StatusCode(500, "An error occurred while stopping background task");
            }
        }


        [HttpPost("stopWatching")]
        public async Task<IActionResult> StopWatching()
        {
            try
            {
                _logger.LogInformation("Stopping directory watching...");
                await _dirWatcherService.StopWatching();
                return Ok("Watching stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while stopping watching");
                return StatusCode(500, "An error occurred while stopping watching");
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            try
            {
                var taskRunDetail = await _dirWatcherService.StartAsync();
                return Ok(taskRunDetail);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "An unexpected error occurred during background task execution.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}