using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebApplication1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HtmlModel model = new HtmlModel();
        public NewBooking bookingInfo = new NewBooking();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Load page

        public async Task<IActionResult> NewDetail(string code)
        {    
            if (code != null)
            {
                //get booking info
                model.BookingHtmlList = await FollowUpCall("BOOKING_CODE", "ND3_COCTEST_GetOneBookingFormItem_Khai", code);
                bookingInfo.code = code;
                model.code = code;
            }

            model.AreaHtmlList = await OnGet("DC_getArea");
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            List<string> list = await OnGet("DC_getInfoBooking");
            return View(list);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Send request to COC

        [HttpPost]
        public async Task<IActionResult> Update(string cd, string code, string cont, string location)
        {
            var response = await UpdateOrCreate(cd, "ND3_COCTEST_DC_addBookingV2_Khai", code, cont, location);
            return Json("temp");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string cd, string cont, string location)
        {
            var bookingId = await OnGet("ND3_COCTEST_Auto_Create_DCKB_Khai");
            var response = await UpdateOrCreate(cd, "ND3_COCTEST_DC_addBookingV2_Khai", bookingId[0], cont, location);
            return Json("temp");
        }

        [HttpPost]
        public IActionResult Refresh()
        {
            model.DistrictHtmlList = new List<string>();
            return View(model);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Button get data

        [HttpPost]
        public async Task<List<string>> clickArea(string para)
        {
            model.ProvinceHtmlList = await FollowUpCall("AREA_CD", "DC_getProvinceFollowArea", para);
            return model.ProvinceHtmlList;
        }

        [HttpPost]
        public async Task<IActionResult> clickFirst(string order, string id)
        {
            List<string> defaultList = new List<string>();
            if (order == "1")
            {
                // province
                defaultList = await FollowUpCall("AREA_CD", "DC_getProvinceFollowArea", id);
                return Json(defaultList[0]);
            }
            if (order == "2") 
            {
                // district
                defaultList = await FollowUpCall("PROVINCE_CD", "DC_getDistrictFollowProvinceNewData", id);
                return Json(defaultList[0]);
            }

            defaultList = await OnGet("DC_getArea");
            return Json(defaultList[0]);
        }

        [HttpPost]
        public async Task<List<string>> clickProvince(string para)
        {
            model.DistrictHtmlList = await FollowUpCall("PROVINCE_CD", "DC_getDistrictFollowProvinceNewData", para);
            bookingInfo.province = para;
            return model.DistrictHtmlList;
        }

        [HttpPost]
        public IActionResult clickDistrict(string para)
        {
            bookingInfo.district = para;
            return View(model);
        }

        [HttpPost]
        public IActionResult clickCont(string para)
        {
            bookingInfo.cont = para;
            return View(model);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // COC call
        public async Task<List<string>> FollowUpCall(string cd, string HeaderValue, string para)
        {
            List<string> list = new List<string>();
            string url = "http://5sm.uat.byover.com:57857/5SAPI/HP";
            string username = "59638228311456";
            string password = "1ca89990ac874aa3";

            string additionalHeaderKey = "Func";
            string additionalHeaderValue = HeaderValue;
            string jsonBody = @"{""USER_CODE"": """",
                                ""USER_TYPE"": """",
                                ""Data"": ""[{\""" + cd + @"\"":\""" + para + @"\""}]""}";

            using (var client = new HttpClient())
            {
                // Add authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Add additional header
                client.DefaultRequestHeaders.Add(additionalHeaderKey, additionalHeaderValue);

                // Set content type to JSON
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create content from JSON body
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response
                    dynamic data = JsonConvert.DeserializeObject(responseContent);


                    string jsonData = data.Data;

                    // Deserialize the Data field into a list of objects
                    List<dynamic> results = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                    if (results == null)
                    {
                        return null;
                    }

                    string htmlContent = "";

                    foreach (var item in results)
                    {
                        if (cd == "BOOKING_CODE")
                        {
                            htmlContent += item.Column1;
                        }
                        else
                        {
                            htmlContent += item.RESULT;
                        }
                        
                        list.Add(htmlContent);
                        htmlContent = "";
                    }

                    return list;
                }
                else
                {
                    // Handle error
                    return list;
                }
            }
        }

        public async Task<IActionResult> UpdateOrCreate(string cd, string HeaderValue, string code, string cont, string location)
        {
            string url = "http://5sm.uat.byover.com:57857/5SAPI/HP";
            string username = "59638228311456";
            string password = "1ca89990ac874aa3";

            string additionalHeaderKey = "Func";
            string additionalHeaderValue = HeaderValue;
            string jsonBody = @"{""USER_CODE"": """",
                                ""USER_TYPE"": """",
                                ""Data"": ""[{\""DATE_VERHICLE_OUT\"": \""\"",
                                            \""NUMBER_VERHICLE\"": \""\"",
                                            \""VENDOR\"": \""\"",
                                            \""NUMBER_CONT\"": \""\"",
                                            \""DESTINATION\"": \""\"",
                                            \""RECEIVING_LOCATION\"": \""" + location + @"\"",
                                            \""EMPTY_RELEASE_LOCATION\"": \""\"",
                                            \""TYPE_CONT\"": \""" + cont + @"\"",
                                            \""BOOKING_CODE\"": \""" + code + @"\"",
                                            \""FEE_TRANSPORT\"": \""\"",
                                            \""CUSTOMER_NAME\"": \""\"",
                                            \""CODE_DELIVERY\"": \""\"",
                                            \""NUMBER_CONTAINER\"": \""\"",
                                            \""CUSTOMER_CODE\"": \""\"",
                                            \""DISTRICT_CD\"": \""" + cd + @"\""}]""}";

            using (var client = new HttpClient())
            {
                // Add authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Add additional header
                client.DefaultRequestHeaders.Add(additionalHeaderKey, additionalHeaderValue);

                // Set content type to JSON
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create content from JSON body
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Get the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response
                    dynamic data = JsonConvert.DeserializeObject(responseContent);


                    string jsonData = data.Data;

                    // Deserialize the Data field into a list of objects
                    List<dynamic> results = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                    return Ok();
                }
                else
                {
                    // Handle error
                    return Ok();
                }
            }
        }


        public async Task<List<string>> ExtendScroll(string search, string page)
        {
            List<string> list = new List<string>();
            string url = "http://5sm.uat.byover.com:57857/5SAPI/HP";
            string username = "59638228311456";
            string password = "1ca89990ac874aa3";

            string additionalHeaderKey = "Func";
            string additionalHeaderValue = "DC_getInfoBooking";
            string jsonBody = @"{""USER_CODE"": """",
                                ""USER_TYPE"": """",
                                ""Data"": ""[{\""PARAM\"": \""" + search + @"\"",\""OFFSET\"": \""" + page + @"\""}]""}";

            using (var client = new HttpClient())
            {   
                // Add authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Add additional header
                client.DefaultRequestHeaders.Add(additionalHeaderKey, additionalHeaderValue);

                // Set content type to JSON
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create content from JSON body
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response
                    dynamic data = JsonConvert.DeserializeObject(responseContent);


                    string jsonData = data.Data;

                    // Deserialize the Data field into a list of objects
                    List<dynamic> results = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                    if (results == null)
                    {
                        return null;
                    }

                    string htmlContent = "";

                    foreach (var item in results)
                    {
                        htmlContent += item.RESULT;
                        list.Add(htmlContent);
                        htmlContent = "";
                    }

                    return list;
                }
                else
                {
                    // Handle error
                    return list;
                }
            }
        }


        public async Task<List<string>> OnGet(string HeaderValue)
        {
            // Replace with your actual values
            string url = "http://5sm.uat.byover.com:57857/5SAPI/HP";
            string username = "59638228311456";
            string password = "1ca89990ac874aa3";
            string additionalHeaderKey = "Func";
            string additionalHeaderValue = HeaderValue;
            string jsonBody = @"{""USER_CODE"": """",
                                ""USER_TYPE"": """",
                                ""Data"": """"
                                }";
            List<string> BookingHtmlList = new List<string>();
            using (var client = new HttpClient())
            {
                // Add authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Add additional header
                client.DefaultRequestHeaders.Add(additionalHeaderKey, additionalHeaderValue);

                // Set content type to JSON
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create content from JSON body
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response
                    dynamic data = JsonConvert.DeserializeObject(responseContent);


                    string jsonData = data.Data;

                    // Deserialize the Data field into a list of objects
                    List<dynamic> results = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                    string htmlContent = "";

                    foreach (var item in results)
                    {
                        if (HeaderValue == "ND3_COCTEST_Auto_Create_DCKB_Khai")
                        {
                            htmlContent += item.BOOKING_CODE;
                        }
                        else
                        {
                            htmlContent += item.RESULT;
                        }
                        
                        BookingHtmlList.Add(htmlContent);
                        htmlContent = "";
                    }
                    return BookingHtmlList;
                }
                else
                {
                    // Handle error
                    return BookingHtmlList;
                }
            }
        }
    }
}
