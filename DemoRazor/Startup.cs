using System;
using System.Globalization;
using AspNetCoreRateLimit;
using DemoRazor.Contexts;
using DemoRazor.Jobs;
using DemoRazor.Models;
using DemoRazor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using RazorHtmlEmails.RazorClassLib.Services;

namespace DemoRazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Config = configuration;
        }

        public IConfiguration Config { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            var cookieSettings = Config.GetSection("Cookie");
            services.Configure<CookieSettings>(cookieSettings);
            services.Configure<EmailSettings>(Config.GetSection("Email"));

            services.Configure<IpRateLimitOptions>(Config.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Config.GetSection("IpRateLimitPolicies"));
            services.Configure<ClientRateLimitOptions>(Config.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Config.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddCors();
            services.AddAntiforgery(options =>
            {
                options.FormFieldName = "XsrfFormField";
                options.HeaderName    = "XSRF-TOKEN";
                options.Cookie.Name   = cookieSettings.GetValue<string>("XSRF:Name");
            });

            //services.AddHsts(options =>
            //{
            //    //options.Preload = true;
            //    options.IncludeSubDomains = false;
            //    options.MaxAge = TimeSpan.FromSeconds(30);
            //});

            services.AddScoped<CaptchaVerificationService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc().AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (_, factory) =>
                    factory.Create(typeof(SharedResources));
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ro-RO")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures     = supportedCultures;
                options.SupportedUICultures   = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider() { CookieName = cookieSettings.GetValue<string>("Localization:Name") });
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
            });

            services.AddAuthentication()
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId     = Environment.GetEnvironmentVariable("AUTH_GOOGLE_CLIENT_ID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("AUTH_GOOGLE_CLIENT_SECRET");
                });

            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name         = cookieSettings.GetValue<string>("Auth:Name"); ;
                options.Cookie.MaxAge       = TimeSpan.FromHours(cookieSettings.GetValue<double>("Auth:MaxAge"));
                options.Cookie.IsEssential  = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite     = SameSiteMode.Lax;
                options.LoginPath           = "/account/login";
                options.LogoutPath          = "/account/logout";
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddScoped<AccessorService>();
            services.AddScoped<EmailQueueService>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<ProductService>();

            services.AddAuthorization();

            services.AddRazorPages();
            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash   = true;
                options.LowercaseUrls         = true;
                options.LowercaseQueryStrings = false;
            });

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.Configure<QuartzOptions>(Config.GetSection("Quartz"));
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();

                var jobKey = new JobKey("Email Quartz Job");
                q.AddJob<EmailJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity(jobKey.Name + " trigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromMinutes(1))
                        .RepeatForever()
                    )
                );
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();
            app.UseClientRateLimiting();

            app.UseSecurityHeaders(policies =>
                policies
                    .AddFrameOptionsDeny()
                    .AddXssProtectionBlock()
                    .AddContentTypeOptionsNoSniff()
                    //.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
                    .RemoveServerHeader()
                    .AddReferrerPolicyNoReferrer()
                    .AddContentSecurityPolicyReportOnly(builder =>
                    {
                        builder.AddUpgradeInsecureRequests();
                        builder.AddBlockAllMixedContent();

                        builder.AddDefaultSrc()
                            .Self()
                            .WithNonce()
                            .StrictDynamic();
                        builder.AddConnectSrc()
                            .Self()
                            .WithNonce()
                            .From("https://*.fontawesome.com");
                        builder.AddFontSrc()
                            .Self()
                            .WithNonce()
                            .StrictDynamic()
                            .From("https://fonts.googleapis.com")
                            .From("https://fonts.gstatic.com")
                            .From("https://*.fontawesome.com");
                        builder.AddObjectSrc()
                            .None();
                        builder.AddFormAction()
                            .Self()
                            .From("https://accounts.google.com");
                        builder.AddImgSrc()
                            .Self()
                            .OverHttps()
                            .From("https://www.w3.org");
                        builder.AddScriptSrc()
                            .Self()
                            .WithNonce()
                            .StrictDynamic();
                        builder.AddStyleSrc()
                            .Self()
                            .UnsafeInline()
                            .From("https://fonts.googleapis.com")
                            .From("https://*.fontawesome.com")
                            .From("https://cdn.jsdelivr.net");
                        builder.AddMediaSrc()
                            .OverHttps();
                        builder.AddFrameAncestors()
                            .Self();
                        builder.AddBaseUri()
                            .Self();
                        builder.AddFrameSrc()
                            .Self()
                            .From("https://www.google.com/");
                    })
                    .AddCustomHeader("X-DemoRazor", "=^.^=")
            );

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHttpsRedirection();
                //app.UseHsts();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
