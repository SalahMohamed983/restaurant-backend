using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResturantBusinessLayer.Services.Interfaces;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string to, string subject, string body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _emailService.SendEmailAsync(to, subject,body);
                if (result)
                {
                    return Ok(new { message = "Email sent successfully." });
                }
                return BadRequest(new { message = "Failed to send email. Please check email configuration." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = $"Error sending email: {ex.Message}" });
            }
        }
    }
}