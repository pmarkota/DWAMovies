using DAL.BLModels;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using Newtonsoft.Json;
using System.Text;

namespace MVC.Controllers
{
    public class ApiResponse
    {
        public int TotalItems { get; set; }
        public List<BLVideo> Videos { get; set; }
    }
    public class VideoController : Controller
    {
        private readonly ApiService _apiService;

        public VideoController(ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string apiUrl = $"/api/videos/paged";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl,true);



                var videosResponse = JsonConvert.DeserializeObject<ApiResponse>(apiResponse);
                var videos = videosResponse.Videos;
                ViewData["genres"] = JsonConvert.DeserializeObject<List<DAL.DALModels.Genre>>(await _apiService.GetApiResponseAsync("/api/genres"));
                ViewData["ApiResponse"] = apiResponse;
                return View(videos);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FilterVideos(string videoName, string genreName, int pageSize, int page)
        {
            try
            {
                string apiUrl = $"/api/videos/paged?videoName={videoName}&genreName={genreName}&page={page}&pageSize={pageSize}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl,true);

                var videosResponse = JsonConvert.DeserializeObject<ApiResponse>(apiResponse);
                var videos = videosResponse.Videos;

                ViewData["genres"] = JsonConvert.DeserializeObject<List<DAL.DALModels.Genre>>(await _apiService.GetApiResponseAsync("/api/genres"));

                ViewData["ApiResponse"] = apiResponse;
                return PartialView("VideoPartialView", videos);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View("Index");
            }
        }

        public async Task<IActionResult> Create()
        {

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(BLVideo video)
        {
            try
            {
                string apiUrl = $"/api/videos";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(video), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl, content);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string apiUrl = $"/api/videos/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl,true);

                var imageApiUrl = $"/api/images";
                string imageApiResponse = await _apiService.GetApiResponseAsync(imageApiUrl, false);
                ViewData["images"] = JsonConvert.DeserializeObject<List<DAL.BLModels.BLImage>>(imageApiResponse);

                var video = JsonConvert.DeserializeObject<BLVideo>(apiResponse);

                return View(video);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, /*[FromBody]*/ BLVideo video)
        {
            try
            {
                string apiUrl = $"/api/videos/{id}";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(video), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PutApiResponseAsync(apiUrl, content);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string apiUrl = $"/api/videos/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, requireAuthorization: true);

                var video = JsonConvert.DeserializeObject<BLVideo>(apiResponse);

                return View(video);
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
                string apiUrl = $"/api/videos/{id}";
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
