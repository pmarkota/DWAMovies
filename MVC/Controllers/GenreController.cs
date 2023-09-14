using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MVC.Services;
using DAL.BLModels;
using DAL;
using DAL.DALModels;

namespace MVC.Controllers
{
    public class GenreController : Controller
    {
        private readonly ApiService _apiService;

        public GenreController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                string apiUrl = $"/api/genres/paged?page={page}&pageSize={pageSize}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var genresResponse = JsonConvert.DeserializeObject<PagedApiResponse<BLGenre>>(apiResponse);

                ViewData["ApiResponse"] = apiResponse;
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(genresResponse);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FilteredGenres(int page = 1, int pageSize = 10)
        {
            try
            {
                string apiUrl = $"/api/genres/paged?page={page}&pageSize={pageSize}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var genresResponse = JsonConvert.DeserializeObject<PagedApiResponse<BLGenre>>(apiResponse);

                ViewData["ApiResponse"] = apiResponse;
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return PartialView("GenrePartialView",genresResponse);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return PartialView("GenrePartialView");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BLGenre genre)
        {
            try
            {
                string apiUrl = "/api/genres";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(genre), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl, content);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string apiUrl = $"/api/genres/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var genre = JsonConvert.DeserializeObject<BLGenre>(apiResponse);

                return View(genre);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BLGenre genre)
        {
            try
            {
                string apiUrl = $"/api/genres/{id}";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(genre), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PutApiResponseAsync(apiUrl, content);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string apiUrl = $"/api/genres/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var genre = JsonConvert.DeserializeObject<BLGenre>(apiResponse);

                return View(genre);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, BLGenre genre)
        {
            try
            {
                string apiUrl = $"/api/genres/{id}";
                string apiResponse = await _apiService.DeleteApiResponseAsync(apiUrl);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }


    }
}
