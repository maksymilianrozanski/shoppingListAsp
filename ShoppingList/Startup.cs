using System;
using GroceryClassification;
using LaYumba.Functional;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Newtonsoft.Json.Serialization;
using ShoppingList.Auth;
using ShoppingList.Data;
using ShoppingList.Data.List;

namespace ShoppingList
{
    public class Startup
    {
        public static string PasswordSalt { get; private set; } = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private Either<StartupErrors, string> GetConfigValue(string key)
        {
            var value = Configuration[key];
            if (value is null or "") return new StartupErrors(StartupErrors.StartupError.ReadingConfigurationFailed);
            else return value;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            GetConfigValue("salt").Match(l =>
            {
                Console.WriteLine("Reading config value failed. Exiting the app.");
                Environment.Exit(1);
            }, r => PasswordSalt = r);

            services.AddTransient<IUserPasswordSalt, UserPasswordSaltImpl>();
            
            services.AddDbContext<ShoppingListDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("ShoppingListConnection"));
                // opt.ConfigureWarnings(w =>
                // w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
            });

            services.AddPredictionEnginePool<GroceryData, GroceryItemPrediction>()
                .FromFile(modelName: "GroceryModel", filePath: "MLModels/model.zip",
                    watchForChanges: false);

            services
                .AddTransient<
                    IUserService<UserLoginData,
                        IdBasedAuthenticationHandler.User>, IdBasedAuthenticationHandler.UserServiceImpl>();
            services.AddTransient<IdBasedAuthenticationHandler>();

            services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", config =>
                {
                    config.Cookie.Name = "UserLoginCookie";
                    config.LoginPath = "/LoginPage";
                    config.LogoutPath = "/Logout";
                });

            services.AddControllers();
            services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/Protected"));


            services.AddControllers().AddNewtonsoftJson(s =>
            {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            /*
             * use SqlShoppingListRepo for default usage
             * use SqlShoppingListRepoExampleData for inserting example data if ShopWaypointsEntities table is empty
             */
            // services.AddTransient<IShoppingListRepo, SqlShoppingListRepo>();
            services.AddTransient<IShoppingListRepo, SqlShoppingListRepoExampleData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // app.UseStatusCodePages(async context =>
            // {
            //     var response = context.HttpContext.Response;
            //
            //     if (response.StatusCode == (int) HttpStatusCode.Unauthorized ||
            //         response.StatusCode == (int) HttpStatusCode.Forbidden)
            //         response.Redirect("/LoginPage");
            // });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        public sealed class StartupErrors
        {
            private readonly StartupError _error;

            public StartupErrors(StartupError error)
            {
                this._error = error;
            }

            public enum StartupError
            {
                ReadingConfigurationFailed
            }
        }
    }
}