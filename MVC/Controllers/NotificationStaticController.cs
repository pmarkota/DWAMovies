using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using System;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class NotificationStaticController : Controller
    {
        private readonly ApiService _apiService;

        public NotificationStaticController(ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string apiUrl = "/api/notifications/unsent/count"; 
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);
                ViewData["ApiResponse"] = apiResponse;
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }
        public async Task<IActionResult> SendAll()
        {
            try
            {
                HttpContent content = null;
                string apiUrl = "/api/notifications/send"; 
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl,content);
                ViewData["ApiResponse"] = apiResponse;

                var notifications = apiResponse.Split('\n');
                ViewBag.Notifications = notifications; 

            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                Console.WriteLine(e.Message);
            }

            return View(); 
        }

    }
}
