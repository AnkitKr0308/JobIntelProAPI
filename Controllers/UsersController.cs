using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobIntelPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {

        }
    }
}
