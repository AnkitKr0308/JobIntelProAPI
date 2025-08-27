using JobIntelPro_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobIntelPro_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JobIntelPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;

        public SubscriptionController(AppDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] Subscriptions subscription)
        {
            try
            {
                if (subscription == null || string.IsNullOrWhiteSpace(subscription.Email))
                {
                    return BadRequest(new { success = false, message = "Email is required" });
                }
                var existingSubscription = await _context.Subscriptions.AnyAsync
                    (s => s.Email == subscription.Email);
                if (existingSubscription)
                {
                    return Conflict(new { success = false, message = "Email is already subscribed" });
                }
                var subscriptionData = new Subscriptions
                {
                    Email = subscription.Email,
                    CreatedAt = DateTime.Now,
                    Verified = true
                };
                _context.Subscriptions.Add(subscriptionData);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Subscribed successfully", email = subscriptionData.Email });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
