using JobIntelPro_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobIntelPro_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;


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
                    Country = job.Country ?? null,
                    City = job.City ?? null,
                    Company = job.Company ?? null,
                    WorkType = job.WorkType ?? null,
                    Experience = job.Experience ?? null,
                    Batch = job.Batch ?? null,
                    Degree = job.Degree ?? null,
                    ApplyURL = job.ApplyURL ?? null,
                    IsActive = job.IsActive,
                    Skills = job.Skills ?? null,
                    CompanyDescription = job.CompanyDescription ?? null,
                    Salary = job.Salary ?? null,
                    Responsibilities = job.Responsibilities ?? null,
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
                var jobs = await _context.Jobs.OrderByDescending(j=>j.UpdatedAt).ToListAsync();
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

        [HttpGet("job/{id}")]
        public async Task<IActionResult> GetJobById([FromRoute] int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    return NotFound(new { success = false, message = "Job not found" });
                }
                return Ok(new { success = true, job });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = errorMessage });
            }
        }


        [HttpGet("searchjobs")]
        public async Task<IActionResult> SearchJobs(
            [FromQuery] string? query,
            [FromQuery] string? country,
            [FromQuery] string? city)
        {
            try
            {
                var searchParam = new SqlParameter("@searchParam",
                string.IsNullOrWhiteSpace(query) ? (object)DBNull.Value : $"%{query}%");

                var countryParam = new SqlParameter("@countryParam",
                    string.IsNullOrWhiteSpace(country) ? (object)DBNull.Value : country);

                var cityParam = new SqlParameter("@cityParam",
                    string.IsNullOrWhiteSpace(city) ? (object)DBNull.Value : city);

                var sql = "EXEC sp_SearchJobs @searchParam, @countryParam, @cityParam";

                var jobs = await _context.Jobs
                    .FromSqlRaw(sql, searchParam, countryParam, cityParam)
                    .ToListAsync();

                if (jobs == null || jobs.Count == 0)
                {
                    return Ok(new { success = true, jobs = new List<Jobs>() });
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





        [HttpGet("countries")]
        public async Task<IActionResult> GetDistinctCountries()
        {
            try
            {
                var countries = await _context.Jobs
                    .Where(j => !string.IsNullOrEmpty(j.Country) && j.Country !=null)
                    .Select(j => j.Country)
                    .Distinct()
                    .ToListAsync();
                if (countries == null || countries.Count == 0)
                {
                    return NotFound(new { success = false, message = "No countries found" });
                }
                return Ok(new { success = true, countries });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = errorMessage });
            }
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCitiesByCountry([FromQuery] List<string>? countries)
        {
            try
            {
                IQueryable<Jobs> query = _context.Jobs;

                if (countries != null && countries.Any())
                {
                    query = query.Where(j => countries.Contains(j.Country) && !string.IsNullOrEmpty(j.City));
                }
                else
                {
                    query = query.Where(j => !string.IsNullOrEmpty(j.City));
                }

                var cities = await query
                    .Select(j => j.City)
                    .Distinct()
                    .ToListAsync();

                if (cities == null || cities.Count == 0)
                {
                    return NotFound(new { success = false, message = "No cities found" });
                }

                return Ok(new { success = true, cities });
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
