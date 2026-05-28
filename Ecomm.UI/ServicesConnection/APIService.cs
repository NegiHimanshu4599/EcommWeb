using Ecomm.UI.Common;
using Ecomm.UI.Models.AuthDtos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ecomm.UI.ServicesConnection
{
    public class APIService : IAPIService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public APIService(IHttpClientFactory clientFactory,
                          IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task<HttpClient> CreateClient(string token = null)
        {
            await RefreshTokenIfNeeded();
            var client = _clientFactory.CreateClient();
            if (string.IsNullOrEmpty(token))
                token = _httpContextAccessor.HttpContext?.Session.GetString("JWT");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
        public async Task<T> GetAsync<T>(string url, string token = null)
        {
            var client = await CreateClient(token);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR | Status: {response.StatusCode} | Message: {error}");
            }
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return default;
            return JsonSerializer.Deserialize<T>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        public async Task<T> PostAsync<T>(string url, object data, string token = null)
        {
            var client = await CreateClient(token);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
            //var error = await response.Content.ReadAsStringAsync();
            //throw new Exception($"API ERROR: {response.StatusCode} - {error}");
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(result))
                return default;
            return JsonSerializer.Deserialize<T>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<T> PutAsync<T>(string url, object data, string token = null)
        {
            var client = await CreateClient(token);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(result))
                return default;
            return JsonSerializer.Deserialize<T>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task DeleteAsync(string url, string token = null)
        {
            var client = await CreateClient(token);
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        public async Task<bool> deleteAsync(string url, string token = null)
        {
            var client = await CreateClient(token);
            var response = await client.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        public async Task<T?> PostMultipartAsync<T>(string url, object dto, string token = null)
        {
            var client = await CreateClient(token);
            using var content = new MultipartFormDataContent();
            foreach (var prop in dto.GetType().GetProperties())
            {
                if (prop.Name == "ImageFile")
                    continue;
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    content.Add(new StringContent(value.ToString()), prop.Name);
                }
            }
            var imageProp = dto.GetType().GetProperty("ImageFile");
            if (imageProp != null)
            {
                var file = imageProp.GetValue(dto) as IFormFile;
                if (file != null)
                {
                    var stream = file.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(streamContent, "ImageFile", file.FileName);
                }
            }
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(result))
                return default;
            return JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        public async Task<bool> PutMultipartAsync(string url,object dto,string token = null)
        {
            var client = await CreateClient(token);
            using var content = new MultipartFormDataContent();
            foreach (var prop in dto.GetType().GetProperties())
            {
                if (prop.Name == "ImageFile")
                    continue;
                var value = prop.GetValue(dto);
                if (value != null)
                {
                    content.Add( new StringContent(value.ToString()),  prop.Name);
                }
            }
            var imageProp = dto.GetType().GetProperty("ImageFile");
            if (imageProp != null)
            {
                var file = imageProp.GetValue(dto) as IFormFile;
                if (file != null)
                {
                    var stream = file.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add( streamContent,"ImageFile", file.FileName);
                }
            }
            var response = await client.PutAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        private async Task RefreshTokenIfNeeded()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo > DateTime.UtcNow)
                return;
            var refreshToken =
                _httpContextAccessor.HttpContext?.Session.GetString( "RefreshToken");
            if (string.IsNullOrEmpty(refreshToken))
                return;
            var client = _clientFactory.CreateClient();
            var body = JsonSerializer.Serialize(new { RefreshToken = refreshToken });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(SD.AuthAPIPath + "/refresh", content);
            if (!response.IsSuccessStatusCode)
                return;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponseDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _httpContextAccessor.HttpContext.Session.SetString("JWT", result.AccessToken);
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", result.RefreshToken);
        }
        public async Task PatchAsync(string url,object? data = null, string? token = null)
        {
            var client = await CreateClient(token);
            HttpContent? content = null;
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                content = new StringContent(json,Encoding.UTF8,"application/json");
            }
            var response = await client.PatchAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception( await response.Content.ReadAsStringAsync());
            }
        }
    }
}