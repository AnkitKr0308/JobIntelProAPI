using JobIntelPro_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobIntelPro_API.Models;
using Microsoft.EntityFrameworkCore;


namespace JobIntelPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;

        public JobsController(AppDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("createjob")]
        public async Task<IActionResult> CreateJob([FromBody] Jobs job)
        {
            try
            {
                if (job == null || string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description))
                {
                    return BadRequest(new { success = false, message = "Title and description are required" });
                }

                var jobData = new Jobs
                {
                    Title = job.Title,
                    Description = job.Description,
                    Country = job.Country ?? "NULL",
                    City = job.City ?? "NULL",
                    Company = job.Company ?? "NULL",
                    WorkType = job.WorkType ?? "NULL",
                    Experience = job.Experience ?? "NULL",
                    Batch = job.Batch ?? "NULL",
                    Degree = job.Degree ?? "NULL",
                    ApplyURL = job.ApplyURL ?? "NULL",
                    IsActive = job.IsActive,
                    Skills = job.Skills ?? "NULL",
                    CompanyDescription = job.CompanyDescription ?? "NULL",
                    Salary = job.Salary ?? "NULL",
                    Responsibilities = job.Responsibilities ?? "NULL",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.Jobs.Add(jobData);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Job created successfully", jobId = jobData.Id, title = jobData.Title });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = errorMessage });
            }

        }

        [HttpGet("alljobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _context.Jobs.ToListAsync();
                if (jobs == null || jobs.Count == 0)
                {
                    return NotFound(new { success = false, message = "No jobs found" });
                }
                return Ok(new { success = true, jobs });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = errorMessage });
            }
        }
    }
}
