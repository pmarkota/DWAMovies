using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        //public async Task<string> GetApiResponseAsync(string apiUrl)
        //{
        //    try
        //    {
        //        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadAsStringAsync();
        //    }
        //    catch (HttpRequestException)
        //    {
        //        throw new Exception("API request failed.");
        //    }
        //}
        public async Task<string> GetApiResponseAsync(string apiUrl, bool requireAuthorization = true)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                if (requireAuthorization == true)
                {
                    AddBearerTokenToRequest(request); // Add the JWT token to the request
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e )
            {
                await Console.Out.WriteLineAsync(e.Message);
                throw new Exception("API request failed.");
            }
        }

        public async Task<string> PostApiResponseAsync(string apiUrl, HttpContent? content)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("API request failed.");
            }
        }
        public async Task<string> DeleteApiResponseAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("API request failed.");
            }
        }

        public async Task<string> PutApiResponseAsync(string apiUrl, HttpContent? content)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("API request failed.");
            }
        }
        private void AddBearerTokenToRequest(HttpRequestMessage request)
        {
            // Retrieve the JWT token from your authentication mechanism.
            //var jwtToken = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "IsAllowed")?.Value;
            var jwtToken = _httpContextAccessor.HttpContext.Session.GetString("AuthToken");
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtTokenObject = tokenHandler.ReadJwtToken(jwtToken);
            var usernameClaim = jwtTokenObject.Claims.FirstOrDefault(c => c.Type == "IsAllowed")?.Value;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }
    }
}
