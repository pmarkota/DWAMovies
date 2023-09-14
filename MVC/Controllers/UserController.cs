using DAL.BLModels;
using DAL;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using Newtonsoft.Json;
using System.Drawing.Printing;
using DAL.DALModels;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Controllers
{

    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UserRegistrationViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
    public class UserLoginViewModel
    {
        public string Username { get; set; } // Username or email for login
        public string Password { get; set; } // Password for login
    }
    public class UserController : Controller
    {
        private readonly ApiService _apiService;

        public UserController(ApiService apiService)
        {
            _apiService = apiService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(UserRegistrationViewModel user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                string apiUrl = "/api/users/register";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl, content);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }
            return View(nameof(Register));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginViewModel user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                string apiUrl = "/api/users/login";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string apiResponse = await _apiService.PostApiResponseAsync(apiUrl, content);

                var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(apiResponse);
                string token = tokenResponse["token"];
                HttpContext.Session.SetString("AuthToken", token);
                Response.Cookies.Append("AuthToken", token, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(11)
                }); Console.WriteLine("Token set in session: " + token);

                return RedirectToAction(nameof(VideoSelectionIndex));
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }
            return View(nameof(Login));
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> VideoSelectionIndex()
        {
            try
            {
                string apiUrl = $"/api/videos";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, true);

                var videosResponse = JsonConvert.DeserializeObject<List<BLVideo>>(apiResponse);
                List<BLImage> images = new List<BLImage>();
                foreach (var video in videosResponse)
                {
                    if (video.ImageId.HasValue)
                    {
                        string imageApiUrl = $"/api/images/{video.ImageId}";
                        string imageApiResponse = await _apiService.GetApiResponseAsync(imageApiUrl, false);
                        var image = JsonConvert.DeserializeObject<BLImage>(imageApiResponse);
                        images.Add(image);
                        // Do something with the image content if needed
                    }


                }
                ViewData["images"] = images;
                ViewData["ApiResponse"] = apiResponse;
                return View(videosResponse);


            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();
        }

        public async Task<IActionResult> VideoDetails(int id)
        {
            try
            {
                string apiUrl = $"/api/videos/{id}";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var video = JsonConvert.DeserializeObject<BLVideo>(apiResponse);

                string imageApiUrl = $"/api/images/{video.ImageId}";
                string imageApiResponse = await _apiService.GetApiResponseAsync(imageApiUrl, false);
                var image = JsonConvert.DeserializeObject<BLImage>(imageApiResponse);

                ViewData["image"] = image;

                return View(video);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }
            return View();
        }

        public async Task<IActionResult> UserDetails()
        {
            try
            {
                string apiUrl = $"/api/users";
                string apiResponse = await _apiService.GetApiResponseAsync(apiUrl, false);

                var usersResponse = JsonConvert.DeserializeObject<List<BLUser>>(apiResponse);
                ViewData["ApiResponse"] = apiResponse;
                ViewData["Users"] = usersResponse;
                return View(usersResponse);
            }
            catch (Exception e)
            {
                ViewData["ApiError"] = "An error occurred: " + e.Message;
            }

            return View();

        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var httpClient = new HttpClient();
                var jwtToken = HttpContext.Session.GetString("AuthToken");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                string apiUrl = @"http://localhost:5102/api/users/change-password";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(PasswordChanged));
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while changing the password.");
                    return View(model);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while changing the password: " + e.Message);
                return View(model);
            }
        }

        [HttpGet("password-changed")]
        public IActionResult PasswordChanged()
        {
            return View();
        }
    }
}

