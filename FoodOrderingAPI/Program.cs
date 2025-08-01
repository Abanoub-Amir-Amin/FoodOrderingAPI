using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace FoodOrderingAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtSettings = builder.Configuration.GetSection("Jwt");

            // Add MVC services with views support
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<IAddressRepo, AddressRepo>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevClient", policy =>
                    policy.WithOrigins("https://localhost:7060", "http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            });

            //Register controllers with JSON options
            builder.Services.AddControllers()
                   .AddJsonOptions(opts =>
                   {
                       opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                       opts.JsonSerializerOptions.MaxDepth = 64;
                   });

            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IDeliveryManService, DeliveryManService>();
            builder.Services.AddScoped<IDeliveryManRepository, DeliveryManRepository>();
            builder.Services.AddSignalR();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddDbContext<ApplicationDBContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("FoodOrderingDb")));

            builder.Services.AddIdentity<User, IdentityRole<string>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; // Set false if no email confirmation in dev
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["AuthToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RestaurantOnly", policy => policy.RequireRole("Restaurant"));
                // Add more role policies if needed
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in the format: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Bearer"
                    },
                    Scheme = "Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRolesAsync(services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseStaticFiles();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseStaticFiles();
            }

            app.Urls.Clear();
            app.Urls.Add("https://localhost:7060");

            app.UseHttpsRedirection();

            app.UseCors("AllowAngularDevClient");

            app.UseAuthentication();
            app.UseAuthorization();

            // Setup endpoints to handle both APIs and MVC routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

            app.MapControllers();

            app.MapHub<ChatHub>("/chathub");
            app.MapHub<NotificationHub>("/notificationhub");

            await app.RunAsync();
        }


        /// Seeds default roles into the system if they don't exist.
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();

            string[] roleNames = { "Admin", "Restaurant", "Customer", "DeliveryMan" };

            foreach (var roleName in roleNames)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));

                }
            }
        }
    }
}
