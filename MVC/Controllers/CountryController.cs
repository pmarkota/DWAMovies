using DAL.DALModels;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;
using Newtonsoft.Json;

namespace MVC.Controllers
{
    public class CountryController : Controller
    {
        private readonly ApiService _apiService;

        public CountryController(ApiService apiService)
        {
            _apiService = apiService;
        }


        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                string apiUrl = $"/api/countries/paged?page={page}&pageSize={pageSize}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl,false);

                var countriesResponse = JsonConvert.DeserializeObject<List<Country>>(apiResponse);

                ViewData["ApiResponse"] = apiResponse;
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(countriesResponse);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

    }
}
