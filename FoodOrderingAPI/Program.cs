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

            // Register user password hasher and JWT token service
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<INotificationRepo, NotificationRepo>();

            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<ICustomerServices, CustomerService>();

            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IShoppingCartServices, ShoppingCartServices>();

            builder.Services.AddScoped<IShoppingCartItemsRepository, ShoppingCartItemsRepository>();
            builder.Services.AddScoped<IShoppingCartIemService, ShoppingCartItemService>();


            builder.Services.AddScoped<IAddressRepo, AddressRepo>();

            builder.Services.AddScoped<IStripeService, StripeService>();

            // Register controllers with JSON options
            builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.MaxDepth = 64;
                });

            // Register Restaurant services and repositories
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

            // Register Admin service and repository
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();

            // Register DeliveryMan services and repositories
            builder.Services.AddScoped<IDeliveryManService, DeliveryManService>();
            builder.Services.AddScoped<IDeliveryManRepository, DeliveryManRepository>();

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepo, OrderRepo>();

            builder.Services.AddScoped<IPromoCodeRepo, PromoCodeRepo>();
            builder.Services.AddScoped<IPromoCodeService, PromoCodeService>();

            builder.Services.AddScoped<IOpenRouteService, OpenRouteService>();//to get duration between two locations

            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IItemRepo, ItemRepo>();

            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<IDiscountRepo, DiscountRepo>();

            builder.Services.AddScoped<IEmailSender, EmailSenderService>();

            builder.Services.AddScoped<IConfirmationEmail, ConfirmationEmail>();

            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReviewRepo, ReviewRepo>();

            builder.Services.AddSignalR();
            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Register EF DbContext
            builder.Services.AddDbContext<ApplicationDBContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("FoodOrderingDb")));

            // Setup Identity with User and Role having string as key
            builder.Services.AddIdentity<User, IdentityRole<string>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });
            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // For dev only; set true in production
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
                }
                ;
            });

            // Register Role-based authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RestaurantOnly", policy => policy.RequireRole("Restaurant"));
                // Add more role policies if needed
            });

            builder.Services.AddSwaggerGen(options =>
            {
                // Define the Bearer authentication scheme to be used in Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in the format: Bearer {your token}"
                });

                // Apply the security scheme globally to all endpoints in Swagger
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                      new OpenApiSecurityScheme
                      {
                         Reference = new OpenApiReference
                         {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer" // Must match the scheme defined above
                         }
                      },
                      Array.Empty<string>() // No specific scopes required
                   }
                });
            });

            // Register OpenAPI/Swagger services
            builder.Services.AddOpenApi();
            // Add CORS policy to allow Angular frontend
            builder.Services.AddCors(options =>
            {
                //options.AddPolicy("public", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
                //op.AddPolicy("subscription", policy =>
                //{
                //    policy.WithOrigins("127.0.0.1").WithHeaders("token", "role").WithMethods("get");
                //});
                options.AddPolicy("AllowAngularDevClient", policy =>
                policy.WithOrigins("http://localhost:4200", "http://localhost:5000") // ✅ ضع هنا الـ Origin الذي يعمل عليه Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
                    });

            // It configures EF Core to use SQL Server as the database provider,
            // and enables support for spatial data types (like geography, geometry) using NetTopologySuite.
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("FoodOrderingDb"),
                    x => x.UseNetTopologySuite() // Enables spatial (GIS) data support
                ));

            var app = builder.Build();

            // Seed default roles before processing requests
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRolesAsync(services);
            }

            // Configure middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles(); // To serve wwwroot content for MVC views, CSS, JS etc.
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            //app.UseHttpsRedirection();
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
