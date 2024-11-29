using InquiryPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InquiryPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WhatsAppController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ContainerInquiry");
        }




        [HttpGet("by-blno/{blNo}")]
        public async Task<IActionResult> GetContainersByBLNo(string blNo)
        {
            if (string.IsNullOrEmpty(blNo))
            {
                return NotFound();
            }

            try
            {
                var results = await _httpClient.GetFromJsonAsync<List<ContainerIndex>>($"/api/Container/by-blno/{blNo}");
                return results != null && results.Any() ? Ok(results) : NotFound();
            }
            catch (Exception)
            {
                return NotFound(); 
            }
        }

        [HttpGet("by-container-no/{containerNo}")]
        public async Task<IActionResult> GetContainersByContainerNo(string containerNo)
        {
            if (string.IsNullOrEmpty(containerNo))
            {
                return NotFound();
            }

            try
            {
                var results = await _httpClient.GetFromJsonAsync<List<ContainerInfo>>($"/api/Container/by-container-no/{containerNo}");
                return results != null && results.Count > 0 ? Ok(results) : NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpGet("amount-by-virno-indexno")]
        public async Task<IActionResult> GetAmountByVirNoOrIndexNo(string virNo, int indexNo)
        {
            if (string.IsNullOrWhiteSpace(virNo) && indexNo <= 0)
            {
                return BadRequest("Either VirNo or a valid IndexNo parameter is required.");
            }

            try
            {
                var url = $"/api/Container/amount-by-virno-indexno?virNo={Uri.EscapeDataString(virNo)}&indexNo={indexNo}";

                // Get the response as a string
                var response = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrEmpty(response))
                {
                    return NotFound();
                }

                // Return the response as JSON
                return Content(response, "application/json");
            }
            catch (JsonException jsonEx)
            {
                // Log JSON deserialization errors
                // LogError(jsonEx); // Uncomment to log
                return BadRequest("Invalid JSON format.");
            }
            catch (HttpRequestException httpEx)
            {
                // Log HTTP request errors
                // LogError(httpEx); // Uncomment to log
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service unavailable.");
            }
            catch (Exception ex)
            {
                // Log other exceptions
                // LogError(ex); // Uncomment to log
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

    }
}
