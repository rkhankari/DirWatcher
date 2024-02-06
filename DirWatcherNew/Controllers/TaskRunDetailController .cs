using DirWatcher.Controllers;
using DirWatcher.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirWatcherNew.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskRunDetailController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TaskRunDetailController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskRunDetail>>> GetTaskRunDetails()
        {
            return await _dbContext.TaskRunDetails.ToListAsync();
        }
    }
}
