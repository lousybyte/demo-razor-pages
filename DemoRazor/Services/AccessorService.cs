using System;
using System.Net.Http;
using System.Threading.Tasks;
using DemoRazor.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DemoRazor.Services
{
    public class AccessorService
    {
        private readonly ILogger<AccessorService> Logger;
        private readonly IConfiguration           Config;
        private readonly AppDbContext             AppDb;
        private readonly IHttpContextAccessor     HttpAccessor;

        public AccessorService(ILogger<AccessorService> logger, IConfiguration config, IHttpContextAccessor httpAccessor, AppDbContext appDbContext)
        {
            Logger       = logger;
            Config       = config;
            HttpAccessor = httpAccessor;
            AppDb        = appDbContext;
        }

        private static readonly HttpClient httpClient = new ();

        public string GetHostAddress() => HttpAccessor.HttpContext?.Request.Scheme + "://" + HttpAccessor.HttpContext?.Request.Host.Host;
        public string GetIp() => HttpAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        private static async Task<bool> IsUrlOnline(string url)
        {
            try
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                var request = new HttpRequestMessage(HttpMethod.Head, url);

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
