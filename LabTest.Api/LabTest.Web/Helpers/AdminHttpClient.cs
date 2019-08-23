using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabTest.Web.Helpers
{
    public static class AdminHttpClient
    {
        private const string JsonMediaType = "application/json";
        private const string BearerKey = "Bearer";
        private const string AuthorizationKey = "Authorization";
        private static readonly List<HttpClient> Clients = new List<HttpClient>();
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        private static HttpClient GetClient(string baseAddress, string token = null)
        {
            if (Clients.Contains(null))
                Clients.RemoveAll(a => a == null);
            var client = Clients.FirstOrDefault(c => c.BaseAddress.AbsoluteUri == baseAddress);
            if (client == null)
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    //CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable),
                    UseDefaultCredentials = true
                };


                //client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
                client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.AcceptEncoding.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                try
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("https://localhost:44363", Assembly.GetCallingAssembly().GetName().Version.ToString()));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerKey, token);
                Clients.Add(client);
            }
            else
            {
                if (!string.IsNullOrEmpty(token) && client.DefaultRequestHeaders.Authorization?.Parameter != token)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerKey, token);
            }

            return client;
        }

        public static void RemoveClient(string baseUrl)
        {
            var client = Clients?.Find(a => a.BaseAddress == new Uri(baseUrl));
            if (client != null)
            {
                client.Dispose();
                Clients.Remove(client);
            }
        }

        //public static async Task<T> GetAsync<T>(string baseAddress, string prmsUrl, HttpRequestBase request, bool autoDispose = false, bool cache = false)
        //{
        //    return await GetAsync<T>(baseAddress, prmsUrl, GetToken(request), autoDispose, cache);
        //}

        public static async Task<T> GetAsync<T>(string baseAddress, string prmsUrl, HttpRequest request, bool autoDispose = false, bool cache = false)
        {
            return await GetAsync<T>(baseAddress, prmsUrl, GetToken(request), autoDispose, cache);
        }

        public static async Task<T> GetAsync<T>(string baseAddress, string prmsUrl, string token, bool autoDispose = false, bool cache = false)
        {
            T output;
            var key = string.Empty;
            if (cache)
            {
                key = $"{baseAddress}{prmsUrl}".ToMD5HashString();
                output = LabtestMemoryCache.Get<T>(key);
                if (output != null)
                {
                    return output;
                }
            }

            var client = GetClient(baseAddress, token);

            var response = await client.GetAsync(prmsUrl);
            //WriteToLog(await GetResponseText(response, $"{baseAddress}{prmsUrl}", "HTTP GET"));
            output = response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            if (cache && output != null)
                LabtestMemoryCache.Set(key, output, DateTimeOffset.Now.AddHours(1));

            if (autoDispose)
            {
                Clients.Remove(client);
                client.Dispose();
            }

            return output;
        }

        public static async Task<HttpResponseMessage> GetAsync(string baseAddress, string prmsUrl, HttpRequest request, bool autoDispose = false)
        {
            var client = GetClient(baseAddress, GetToken(request));
            var response = await client.GetAsync(prmsUrl);
            //WriteToLog(await GetResponseText(response, $"{baseAddress}{prmsUrl}", "HTTP GET"));
            if (autoDispose)
            {
                Clients.Remove(client);
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> PostAsync(string baseAddress, string extraUrl, object obj, HttpRequest request, bool autoDispose = false)
        {
            var client = GetClient(baseAddress, GetToken(request));
            var response = await client.PostAsync(extraUrl, new StringContent(JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, JsonMediaType));
            //WriteToLog(await GetResponseText(response, $"{baseAddress}{extraUrl}", "HTTP POST"));
            if (autoDispose)
            {
                Clients.Remove(client);
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> PostAsync(string baseAddress, string extraUrl, IDictionary<string, string> obj, HttpRequest request, bool autoDispose = false)
        {
            var client = GetClient(baseAddress, GetToken(request));
            var response = await client.PostAsync(extraUrl, new FormUrlEncodedContent(obj));
            //WriteToLog(await GetResponseText(response, $"{baseAddress}{extraUrl}", "HTTP POST"));
            if (autoDispose)
            {
                Clients.Remove(client);
                client.Dispose();
            }
            return response;
        }

        public static async Task<string> GetStringAsync(string baseAddress, string prmsUrl, HttpRequest request, bool autoDispose = false)
        {
            var res = await GetAsync(baseAddress, prmsUrl, request, autoDispose);
            //WriteToLog(await GetResponseText(res, $"{baseAddress}{prmsUrl}", "HTTP GET"));
            return res.IsSuccessStatusCode ? await res.Content.ReadAsStringAsync() : null;
        }

        public static async Task<HttpResponseMessage> PostAsync(string baseAddress, string extraUrl, MultipartFormDataContent obj, HttpRequest request, bool autoDispose = false)
        {
            var client = GetClient(baseAddress, GetToken(request));
            var res = await client.PostAsync(extraUrl, obj);
            //WriteToLog(await GetResponseText(res, $"{baseAddress}{extraUrl}", "HTTP POST"));
            if (autoDispose)
            {
                Clients.Remove(client);
                client.Dispose();
            }
            return res;
        }

        public static async Task<HttpResponseMessage> PutAsync(string baseAddress, string extraUrl, object obj, HttpRequest request)
        {
            var client = GetClient(baseAddress, GetToken(request));
            return await client.PutAsync(extraUrl, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, JsonMediaType));
        }

        public static async Task<HttpResponseMessage> PutAsync(string baseAddress, string extraUrl, object obj, string token)
        {
            var client = GetClient(baseAddress, token);
            return await client.PutAsync(extraUrl, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, JsonMediaType));
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string baseAddress, string extraUrl, object obj, string token)
        {
            var client = GetClient(baseAddress, token);
            return await client.DeleteAsync(extraUrl);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string baseAddress, string extraUrl, object obj, HttpRequest request)
        {
            var client = GetClient(baseAddress, GetToken(request));
            return await client.DeleteAsync(extraUrl);
        }

        private static string GetToken(HttpRequest request)
        {
            var cookie = request.Cookies["Auth"];
            if (cookie != null)
            {
                var token = cookie;
                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }
            }
            return request.Headers[AuthorizationKey].ToString()?.Replace(BearerKey, string.Empty).Trim();
        }

       
        private static async Task<string> GetResponseText(HttpResponseMessage response, string url, string type)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("==============================Begin Response Message=========================================");
            builder.Append("Request Uri: ");
            builder.AppendLine(url);
            builder.Append("Request Method: ");
            builder.AppendLine(type);
            builder.AppendLine(response.ToString());
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content?.Headers?.ContentLength > 0)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    builder.AppendLine("Content:").AppendLine(error.ToUnscapeString());
                }
            }
            builder.AppendLine("==============================End Response Message==========================================").AppendLine();
            return builder.ToString();
        }

        public static string ToMD5HashString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            using (var md5 = MD5.Create())
            {
                md5.Initialize();
                return string.Join(string.Empty, md5.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(a => a.ToString("x2")));
            }
        }
        public static string ToUnscapeString(this string input)
        {
            return Regex.Unescape(input);          
        }

    }
}
