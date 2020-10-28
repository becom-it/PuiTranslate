using Microsoft.Extensions.Logging;
using PuiTranslate.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PuiTranslate.Services
{
    public interface ISQLService
    {
        void SetAuth(string base64);
        Task<bool> Excute(string sql, string base64 = "");
        Task<List<T>> Query<T>(string sql, string base64 = "");
        Task<T> QuerySingle<T>(string query, string base64 = "");
    }

    public class SQLService : ISQLService
    {
        private readonly ILogger<SQLService> _logger;
        private readonly HttpClient _httpClient;
        private string _base64;

        public SQLService(ILogger<SQLService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> Excute(string sql, string base64 = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(base64)) _base64 = base64;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", _base64);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var query = new QueryModel
                {
                    Query = sql
                };
                var json = JsonSerializer.Serialize(query);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                requestMessage.Content = content;

                var res = await _httpClient.SendAsync(requestMessage);
                if (res.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error calling endpoint. {res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing statement on sql endpoint: {ex.Message}", ex);
            }
        }

        public async Task<List<T>> Query<T>(string sql, string base64 = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(base64)) _base64 = base64;

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", _base64);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var query = new QueryModel
                {
                    Query = sql
                };
                var json = JsonSerializer.Serialize(query);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                requestMessage.Content = content;

                var res = await _httpClient.SendAsync(requestMessage);
                if (res.IsSuccessStatusCode)
                {
                    var payloadStr = await res.Content.ReadAsStringAsync();
                    var resp = JsonSerializer.Deserialize<Response<T>>(payloadStr);
                    return resp.Data;
                }
                else
                {
                    throw new Exception($"Error calling endpoint. {res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error requesting results from sql endpoint: {ex.Message}", ex);
            }
        }

        public async Task<T> QuerySingle<T>(string sql, string base64 = "")
        {
            try
            {
                var res = await Query<T>(sql, base64);
                return res.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error requesting single result from sql endpoint: {ex.Message}", ex);
            }
        }

        public void SetAuth(string base64)
        {
            _base64 = base64;
        }
    }
}
