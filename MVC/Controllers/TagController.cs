using DAL;
using DAL.BLModels;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class TagController : Controller
    {
        private readonly ApiService _apiService;

        public TagController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                string apiUrl = $"/api/tags/paged?page={page}&pageSize={pageSize}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var tagsResponse = JsonConvert.DeserializeObject<PagedApiResponse<BLTag>>(apiResponse);

                ViewData["ApiResponse"] = apiResponse;
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(tagsResponse);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BLTag tag)
        {
            try
            {
                string apiUrl = "/api/tags";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl, content);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string apiUrl = $"/api/tags/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var tag = JsonConvert.DeserializeObject<BLTag>(apiResponse);

                return View(tag);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BLTag tag)
        {
            try
            {
                string apiUrl = $"/api/tags/{id}";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PutApiResponseAsync(apiUrl, content);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string apiUrl = $"/api/tags/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var tag = JsonConvert.DeserializeObject<BLTag>(apiResponse);

                return View(tag);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                string apiUrl = $"/api/tags/{id}";
                string apiResponse = await _apiService.DeleteApiResponseAsync(apiUrl);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }
    }
}
