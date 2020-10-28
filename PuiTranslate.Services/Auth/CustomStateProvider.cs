using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuiTranslate.Common.Models.Auth;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PuiTranslate.Services.Auth
{
    public class CustomStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<CustomStateProvider> _logger;
        private readonly IConfiguration _config;
        private readonly ISQLService _sqlService;
        private readonly ILocalStorageService _localStorage;

        //public CustomStateProvider(ILogger<CustomStateProvider> logger, IConfiguration config, ILocalStorageService localStorage)

        public CustomStateProvider(ILogger<CustomStateProvider> logger, IConfiguration config, ISQLService sqlService, ILocalStorageService localStorage)
        {
            _logger = logger;
            _config = config;
            _sqlService = sqlService;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var payloadStr = await _localStorage.GetItemAsStringAsync("auth");
                if (!string.IsNullOrEmpty(payloadStr))
                {
                    var usr = JsonSerializer.Deserialize<CurrentUser>(payloadStr);
                    var claims = new[] {
                            new Claim(ClaimTypes.Name, usr.UserName),
                            new Claim("Benutzer", usr.Description),
                            new Claim("UserStatus", usr.UserStatus)
                        };
                    identity = new ClaimsIdentity(claims, "Server authentication");

                    _sqlService.SetAuth(usr.AuthString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task Login(LoginRequest loginRequest)
        {
            try
            {
                var base64EncodedAuthenticationString
                    = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{loginRequest.UserName}:{loginRequest.Password}"));
                var sql = "select AUTHORIZATION_NAME, STATUS, TEXT_DESCRIPTION FROM QSYS2.USER_INFO WHERE AUTHORIZATION_NAME = CURRENT USER";

                _sqlService.SetAuth(base64EncodedAuthenticationString);
                var user = await _sqlService.QuerySingle<CurrentUser>(sql);
                user.IsAuthenticated = true;
                user.AuthString = base64EncodedAuthenticationString;
                
                //Abspeichern
                var payloadStr = JsonSerializer.Serialize(user);
                await _localStorage.SetItemAsync("auth", payloadStr);
                
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                
                return;

            } catch(Exception ex)
            {
                _logger.LogError($"Error trying to login: {ex.Message}");
            }
            
            await _localStorage.RemoveItemAsync("auth");
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            //var httpClient = new HttpClient();
            //Uri uri = new Uri(_config["sqlapi"]);
            //Uri uri = new Uri("https://as400test:11443/api/v1/sql/raw");



            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            //requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));



            //var query = new QueryModel
            //{
            //    Query = sql
            //};
            //var json = JsonSerializer.Serialize(query);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //requestMessage.Content = content;

            //var res = await _httpClient.SendAsync(requestMessage);
            //var res = await httpClient.SendAsync(requestMessage);
            //if (res.IsSuccessStatusCode)
            //{
            //var payloadStr = await res.Content.ReadAsStringAsync();
            //var resp = JsonSerializer.Deserialize<Response<CurrentUser>>(payloadStr);
            //if (resp.Data.Count > 0)
            //{
            //var usr = resp.Data.FirstOrDefault();
            //        if (usr != null && usr.UserStatus == "*ENABLED")
            //        {
                        
            //        }
                //}
            //}
            
        }
    }
}
