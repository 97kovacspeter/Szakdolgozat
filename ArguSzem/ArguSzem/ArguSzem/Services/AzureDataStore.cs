using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ArguSzem.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArguSzem.Services
{
    public class AzureDataStore : IDataStore
    {
        private readonly HttpClient _client;

        public bool IsUserLoggedIn { get; private set; }

        public AzureDataStore(string baseAddress)
        {
            IsUserLoggedIn = false;
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        public async Task<bool> RegisterAsync(string email, string password, string confirmPassword)
        {
            var user = new RegisterBindingModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var json = JsonConvert.SerializeObject(user);

            HttpContent content = new StringContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync("https://arguszem.azurewebsites.net/api/Account/Register", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post, "https://arguszem.azurewebsites.net/Token")
            {
                Content = new FormUrlEncodedContent(keyValues)
            };

            var response = await _client.SendAsync(request);

            var jwt = await response.Content.ReadAsStringAsync();

            JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(jwt);

            TokenHandlerModel.AccessToken = jwtDynamic.Value<string>("access_token");

            if (response.IsSuccessStatusCode)
            {
                IsUserLoggedIn = true;
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<bool> LogoutAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHandlerModel.AccessToken);

            var response = await _client.PostAsJsonAsync("https://arguszem.azurewebsites.net/api/Account/Logout", "");

            if (response.IsSuccessStatusCode)
            {
                IsUserLoggedIn = false;
                return true;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<int> GetPortAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenHandlerModel.AccessToken);
            var result = await _client.GetAsync("https://arguszem.azurewebsites.net/api/Port");

            if (result.IsSuccessStatusCode)
            {
                var port = await result.Content.ReadAsStringAsync();

                return int.Parse(port);
            }

            var response = await _client.PostAsync("https://arguszem.azurewebsites.net/api/Port", null);

            if (response.IsSuccessStatusCode)
            {
                var port = await response.Content.ReadAsStringAsync();

                return int.Parse(port);
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }
    }
}