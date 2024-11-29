using InquiryPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace InquiryPortal.Controllers
{
    public class InquiryPortal : Controller
    {
        private readonly ILogger<InquiryPortal> _logger;

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public InquiryPortal(ILogger<InquiryPortal> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;

            _httpClient = httpClientFactory.CreateClient("ContainerInquiry");
        }

        public IActionResult ContainerInquiry()
        {
            if(TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View();
        }

        public async Task<IActionResult> Details(string containerNo,string BLNo)
        {
            if (string.IsNullOrEmpty(containerNo))
            {
                return BadRequest("Container number is required.");
            }

            //var apiUrl = $"/api/Container/details?containerNo={containerNo}&blNo={BLNo}";


            try
            {

                var response = await _httpClient.GetAsync($"/api/Container/details?containerNo={containerNo}&blNo={BLNo}");


                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                   

                    // Deserialize the response
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                    if (result != null)
                    {
                        return Json(new
                        {
                            ContainerNo = result.containerNo?.ToString() ?? "N/A",
                            IGMNo = result.igmNo?.ToString() ?? "N/A",
                            BLNo = result.blNo?.ToString() ?? "N/A",
                            IndexNo = result.indexNo?.ToString() ?? "N/A",
                            Line = result.line?.ToString() ?? "N/A",
                            SizeType = result.sizeType?.ToString() ?? "N/A",
                            Category = result.category?.ToString() ?? "N/A",
                            GoodsDescription = result.goodsDescription?.ToString() ?? "N/A",
                            PortOfDischarge = result.portOfDischarge?.ToString() ?? "N/A",
                            PortDischargeDate = result.portDischargeDate?.ToString() ?? "N/A",
                            ArrivalDate = result.arrivalDate?.ToString() ?? "N/A",
                            DeliverDate = result.deliverDate?.ToString() ?? "N/A",
                            Vessel = result.vessel?.ToString() ?? "N/A",
                            Voyage = result.voyage?.ToString() ?? "N/A",
                            ShipperSeal = result.shipperSeal?.ToString() ?? "N/A",
                            GrossWeight = result.grossWeight?.ToString() ?? "N/A"
                        });
                    }
                    else
                    {
                        return NotFound("No data found for the given container number.");
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error retrieving data: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        public async Task<IActionResult> InquiryPortalByBLNo(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                TempData["ErrorMessage"]  = "BL cannot be null or empty.";
                return RedirectToAction("ContainerInquiry","InquiryPortal");
            }

            List<ContainerIndex> inquiries = new List<ContainerIndex>();

            try
            {
                // Prepare the API URL
                //var apiUrl = $"http://3.64.191.248:8083/api/Container/by-blno/{input}";

                // Call the API
                var results = await _httpClient.GetFromJsonAsync<List<ContainerIndex>>($"/api/Container/by-blno/{input}");



                if (results != null && results.Any())
                {
                    inquiries = results;
                }
                else
                {
                    ViewBag.ErrorMessage = "No data found. Make sure your BL No is correct.";
                    return View();
                }

               
                return View(inquiries); // Pass the list to the view
            }
             catch (Exception)
            {
                // Log the exception if you have logging set up
                // _logger.LogError(ex, "An error occurred while retrieving container details.");
                TempData["ErrorMessage"] = "Invalid BL No.";
                return RedirectToAction("ContainerInquiry", "InquiryPortal");
            }
        }

        public async Task<IActionResult> InquiryByContainerNo(string containerNo)
        {
            if (string.IsNullOrEmpty(containerNo))
            {
                TempData["ErrorMessage"] = "Container No cannot be null or empty.";
                return RedirectToAction("ContainerInquiry", "InquiryPortal");
            }

            //var apiUrl = $"http://3.64.191.248:8083/api/Container/by-container-no/{containerNo}";

            try
            {
                

                var results = await _httpClient.GetFromJsonAsync<List<ContainerInfo>>($"/api/Container/by-container-no/{containerNo}");
                if (results == null || results.Count == 0)
                {
                    return NotFound("No data found for the specified container number.");
                }

                return View(results); // Pass the data to the view
            }
            catch (Exception)
            {
                // Log the exception if you have logging set up
                // _logger.LogError(ex, "An error occurred while retrieving container details.");
                TempData["ErrorMessage"] = "Invalid Container No.";
                return RedirectToAction("ContainerInquiry", "InquiryPortal");
            }
        }
        public class InquiryResult
        {
            public long totalwithgst { get; set; }
            public string TillDate { get; set; } // Add other fields as necessary
        }
        public async Task<IActionResult> BillingInquiry(string VirNO, int IndexNO, string FromDate,string billingType)
              
          {



            if (string.IsNullOrEmpty(VirNO))
            {
                return Json(new { success = false, message = "IGM No  cannot be null or empty." });
            }
            if (IndexNO == null || IndexNO ==0)
            {
                return Json(new { success = false, message = "Index No cannot be null or empty." });
            }
            if (string.IsNullOrEmpty(FromDate))
            {
                return Json(new { success = false, message = "Bill Date cannot be null or empty." });
            }

            try
            {
                DateTime fromDate = DateTime.ParseExact(FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                // Step 3: Format the date as "MM/DD/YYYY" with default time "12:00:00 AM"
                if (billingType== "CY")
                {
                    var url = $"http://192.168.7.35:8092/api/AdvanceBilling/GetList?igm={VirNO}&indexNo={IndexNO}&BillDate={fromDate}";
                    var response = await _httpClient.GetFromJsonAsync<InvoiceInquiry>(url);
                    if (response == null)
                    {
                        return Json(new { success = false, message = "No data found for the specified container number." });
                    }
                    return Json(response);



                }
                else
                {
                    var url = $"http://192.168.7.35:8092/api/AdvanceBilling/GetListCFS?igm={VirNO}&indexNo={IndexNO}&BillDate={fromDate}";
                    var response = await _httpClient.GetFromJsonAsync<InvoiceInquiry>(url);
                    if (response == null)
                    {
                        return Json(new { success = false, message = "No data found for the specified container number." });
                    }
                    return Json(response);

                }

            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, message = $"Request error: {ex.Message}" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }


    }
}
