using System.Net;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using ShoppingList.Auth;
using ShoppingList.Data;

namespace ShoppingList
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
            services.AddDbContext<ShoppingListDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("ShoppingListConnection")));

            // services
            //     .AddAuthentication("CookieAuthentication")
            //     // .AddAuthentication("BasicAuthentication")
            //     // .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
            //         // ("BasicAuthentication", null)
            //     .AddCookie("CookieAuthentication", config =>
            //     {
            //         config.Cookie.Name = "UserLoginCookie";
            //         config.LoginPath = "/LoginPage2";
            //     })
            //     ;
            //
            // services.AddSession(options =>
            // {
            //     options.Cookie.IsEssential = true;
            // });
            services
                .AddTransient<BasicAuthenticationHandler.IUserService, BasicAuthenticationHandler.UserServiceImpl>();
            services.AddTransient<BasicAuthenticationHandler>();
            
            services.AddAuthentication("CookieAuthentication")  
                .AddCookie("CookieAuthentication", config =>  
                {  
                    config.Cookie.Name = "UserLoginCookie";  
                    config.LoginPath = "/LoginPage2";  
                });  
            
            services.AddControllers();
            services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/Protected"));

            

            services.AddControllers().AddNewtonsoftJson(s =>
            {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddTransient<IShoppingListRepo, SqlShoppingListRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            // app.UseStatusCodePages(async context =>
            // {
            //     var response = context.HttpContext.Response;
            //
            //     if (response.StatusCode == (int) HttpStatusCode.Unauthorized ||
            //         response.StatusCode == (int) HttpStatusCode.Forbidden)
            //         response.Redirect("/LoginPage2");
            // });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseWhen(context => context.Request.Path.StartsWithSegments($"/shoppingList"),
            // builder => builder.UseMiddleware<BasicAuthMiddleware>("localhost"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}