using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorCookies.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CookieController : ControllerBase
{
    [HttpPost("save")]
    public IActionResult SaveCookie([FromBody] CookieRequest request)
    {
        try
        {
            var option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append(request.Key, request.Value, option);
            return Ok("Cookie saved");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error saving cookie: {ex.Message}");
        }
    }


    [HttpPost("read-cookie")]
    public IActionResult ReadCookie([FromBody] CookieRequest request)
    {
        try
        {
            var cookieValue = Request.Cookies[request.Key];
            return Ok(cookieValue);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error reading cookie: {ex.Message}");
        }

    }
}

public class CookieRequest
{
    public string Key { get; set; }
    public string? Value { get; set; }
}
