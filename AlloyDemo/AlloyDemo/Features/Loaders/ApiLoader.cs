using AlloyDemo.Models.Loaders;
using AlloyDemo.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace AlloyDemo.Features.Loaders
{
    public class ApiLoader
    {
        private readonly string password;
        private readonly string username;
        private readonly string baseUrl;
        private readonly string userId;
        private readonly string apiKey;

        //probably create a ctor that takes a IConfig of some sort
        //so we can inject the proper creds;
        public ApiLoader()
        {
            //load these from a configuration
            password = "password";
            username = "username";
            baseUrl = "https://auth.actioniq.com";
            userId = "userId";
            apiKey = "apiKey";
        }

        public void LoadJson(IProfile profile)
        {
            string token = Task.Run(async () => await getToken()).Result.ToString();

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/profiles/user?userId={userId}");
            request.Headers.Add("x-api-key", $"{apiKey}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = Task.Run(async () => await client.SendAsync(request)).Result;
            response.EnsureSuccessStatusCode();
            string profileJsonString = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result.ToString();
            ((JsonProfile)profile).LoadJson(profileJsonString);
        }
        public void PopulateProfileFromExternalSource_One(IProfile profile)
        {
            profile["first_name"] = "Deane";
            profile["last_name"] = "Barker";
            profile["dob"] = "1971-09-03";
            profile["annual_salary"] = "10000";
            profile["state"] = "SD";
            profile["country"] = "USA";
            profile["last_visited"] = "2022-12-01";
        }
        private async Task<string> getToken()
        {
            string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/oauth2/token");
            request.Headers.Add("Authorization", $"Basic {encodedStr}");
            request.Headers.Add("Cookie", "XSRF-TOKEN=821e4086-b825-4d80-976b-0107f27d6875");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<TokenResponseModel>(await response.Content.ReadAsStringAsync());

            return result.AccessToken;
            
        }
    }
}