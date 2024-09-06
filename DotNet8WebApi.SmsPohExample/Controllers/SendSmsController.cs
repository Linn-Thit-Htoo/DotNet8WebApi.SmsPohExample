using DotNet8WebApi.SmsPohExample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DotNet8WebApi.SmsPohExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendSmsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SendSmsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendSms([FromBody] SmsDto sms)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.GetSection("AuthKey").Value!);
                string jsonStr = JsonConvert.SerializeObject(sms);
                HttpContent content = new StringContent(jsonStr, Encoding.UTF8, Application.Json);
                HttpResponseMessage response = await _httpClient.PostAsync("/api/v2/send", content);

                string message = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return Ok(message);
                }

                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
