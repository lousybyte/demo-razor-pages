using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DemoRazor.Contexts;
using DemoRazor.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public string GetHostAddress() => HttpAccessor.HttpContext?.Request.Scheme + "://" + HttpAccessor.HttpContext?.Request.Host.Host;
        public string GetIp() => HttpAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        private static bool IsUrlOnline(string url)
        {
            try
            {
                var request     = WebRequest.Create(url);
                request.Proxy   = null;
                request.Method  = "HEAD";
                request.Timeout = 3000;

                var response = request.GetResponse();
                response.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
