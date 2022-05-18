using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LEAVE.DAL.Security;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using LEAVE.BLL.Data;
using LEAVE.BLL.Settings;
using LEAVE.BLL.Security;
using LEAVE.BLL.Leave;
using LEAVE.BLL.Employees;
using LEAVE.BLL.Institutions;
using LEAVE.BLL.Helpers;

namespace LEAVE.DAL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10000;//field count
            });

            services.AddDbContext<LEAVEContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<ApplicationUser, SecurityRole>(options =>
             {
                 //options.SignIn.RequireConfirmedAccount = true;

                 // Lockout settings
                 options.Lockout.AllowedForNewUsers = true;
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                 options.Lockout.MaxFailedAccessAttempts = 5;

                 //simplify passwords
                 options.Password.RequireDigit = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;

                 options.User.RequireUniqueEmail = true;
             })
                .AddEntityFrameworkStores<LEAVEContext>().AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.Name = "LEAVE";
                o.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddScoped<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

            services.AddTransient<IDbRepository, LEAVERepository>();
            services.AddScoped<ISettingsService, SettingsService>();
            //security
            services.AddScoped<ISessionService, SessionService>();
            services.AddSingleton<IPermissionHelperService, PermissionHelper>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IInstitutionService, InstitutionService>();

            //configure email sender
            var emailSettings = Configuration.GetSection("EmailSettings");
            services.AddSingleton(typeof(IEmailSender)
                , new EmailSender(emailSettings.GetValue<bool>("Enabled"),
                emailSettings.GetValue<string>("SmtpHost"),
                emailSettings.GetValue<int>("SmtpPort"),
                emailSettings.GetValue<bool>("UseSsl"),
                emailSettings.GetValue<string>("Username"),
                emailSettings.GetValue<string>("Password")));

            services.AddMvc();

            var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new NoCacheFilter());
                options.Filters.Add(new HandleLockedOutUserFilter());
            })
            .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();//ensure Camel Casing is retained instead of lower case first xter
    }
);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<SecurityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(name: "areas", "areas", pattern: "{area:exists}/{controller=Default}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            var services = app.ApplicationServices.GetService<IServiceScopeFactory>();
            var context = services.CreateScope().ServiceProvider.GetRequiredService<LEAVEContext>();
            context.Database.Migrate();

            IdentityDataInitializer.SeedData(userManager, roleManager);
        }
    }
}
