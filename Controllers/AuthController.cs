using JobIntelPro_API.DTO;
using JobIntelPro_API.Models;
using JobIntelPro_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobIntelPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;

        public AuthController(AppDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpUser([FromBody] Users user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.Name) ||
                    string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest(new { success = false, message = "Name, email, and password are required" });
                }

                if (!user.Email.Contains("@"))
                    return BadRequest(new { success = false, message = "Invalid email address" });

                bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower());
                if (emailExists)
                    return BadRequest(new { success = false, message = "Email already exists" });

                var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new Users
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = hashPassword,
                    Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role,
                    Gender = string.IsNullOrEmpty(user.Gender) ? "NULL" : user.Gender,
                    Contact = string.IsNullOrEmpty(user.Contact) ? "" : user.Contact,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, userId = newUser.Id, name = newUser.Name, role = newUser.Role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDTO login)
        {
            try
            {
                if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                    return BadRequest(new { success = false, message = "Email and password are required" });

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.Email.ToLower());
                if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                    return Unauthorized(new { success = false, message = "Invalid email or password" });

                var token = _jwtService.GenerateJwtToken(user);

                return Ok(new
                {
                    success = true,
                    token,
                    user = new { id = user.Id, email = user.Email, name = user.Name, role = user.Role }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true, message = "Logged out successfully" });
        }
    }
}
