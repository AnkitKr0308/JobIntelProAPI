using Microsoft.AspNetCore.Mvc;
using JobIntelPro_API.Models; 
using System.Threading.Tasks;

namespace JobIntelPro_API.Controllers
{
    [Route("meta")]
    public class JobMetaController : Controller
    {
        private readonly AppDbContext _context;

        public JobMetaController(AppDbContext context)
        {
            _context = context;
        }

        // Meta page for job detail
        [HttpGet("job/{id}")]
        public async Task<IActionResult> JobMeta(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            var html = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='utf-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1' />

                <title>{job.Title} at {job.Company}</title>
                <meta name='description' content='{job.Description?.Substring(0, Math.Min(150, job.Description.Length))}' />

                <!-- Open Graph -->
                <meta property='og:title' content='{job.Title} at {job.Company}' />
                <meta property='og:description' content='{job.Description?.Substring(0, Math.Min(200, job.Description.Length))}' />
                <meta property='og:url' content='https://yourdomain.com/job/{job.Id}' />
                <meta property='og:image' content='https://yourdomain.com/static/job-banner.png' />

                <!-- Twitter -->
                <meta name='twitter:card' content='summary_large_image' />
                <meta name='twitter:title' content='{job.Title} at {job.Company}' />
                <meta name='twitter:description' content='{job.Description?.Substring(0, Math.Min(200, job.Description.Length))}' />
                <meta name='twitter:image' content='https://yourdomain.com/static/job-banner.png' />
            </head>
            <body>
                <h1>{job.Title}</h1>
                <p>{job.Description}</p>
                <script>
                    window.location.href = '/job/{job.Id}'; // redirect humans
                </script>
            </body>
            </html>";

            return Content(html, "text/html");
        }

        
        [HttpGet("jobs")]
        public IActionResult JobsMeta()
        {
            var html = @"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <title>Job Listings</title>
                <meta name='description' content='Browse the latest job opportunities.' />
            </head>
            <body>
                Redirecting...
                <script>window.location.href='/jobs';</script>
            </body>
            </html>";

            return Content(html, "text/html");
        }
    }
}
