using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AuthentikTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecureController : ControllerBase
    {
        // Endpoint accessible to any authenticated user
        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUser()
        {
            return Ok(new { message = "Hello authenticated user" });
        }

        // Endpoint restricted to users in the "admin" role
        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public IActionResult GetAdmin()
        {
            return Ok(new { message = "Hello admin" });
        }

        // Optional example fetching roles/groups from the /userinfo endpoint
        [Authorize]
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized();
            }

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var res = await http.GetAsync("https://auth.hyperc.tr/application/o/userinfo/");
            if (!res.IsSuccessStatusCode)
            {
                return StatusCode((int)res.StatusCode);
            }

            var json = await res.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}
