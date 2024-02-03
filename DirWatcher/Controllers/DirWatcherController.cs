using Microsoft.AspNetCore.Mvc;
using DirWatcher.Models;
using DirWatcher.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DirWatcher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirWatcherController : ControllerBase
    {
        private readonly DirWatcherService _dirWatcherService;
        private readonly ILogger<DirWatcherController> _logger;
        //private readonly IDirectoryWatcher _directoryWatcher;
        public DirWatcherController(DirWatcherService dirWatcherService)
        {
            
            _dirWatcherService = dirWatcherService;
        }
        public DirWatcherController(ILogger<DirWatcherController> logger)
        {
            _logger = logger;
            //_directoryWatcher = directoryWatcher;
        }
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            bool isRunning = _dirWatcherService.IsTaskRunning();
            string status = isRunning ? "Running" : "Stopped";
            return Ok($"Background task status: {status}");
        }

        [HttpPost("start")]
        public IActionResult StartBackgroundTask([FromBody] WatchRequest request)
        {
            _dirWatcherService.StartBackgroundTask(request);
            return Ok("Background task started.");
        }
        /// <summary>
        /// Stops the background task.
        /// </summary>
        /// <returns>A message indicating the background task has stopped.</returns>
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


        /// <summary>
        /// Stops watching the directory.
        /// </summary>
        /// <returns>A message indicating directory watching has stopped.</returns>
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
    }
}