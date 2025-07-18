using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace FoodOrderingAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDBContext>(op =>
            op.UseSqlServer(builder.Configuration.GetConnectionString("FoodOrderingDb")));

            builder.Services.AddSignalR();

            builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();
            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(op =>
            {
                op.SaveToken = true;
                op.RequireHttpsMetadata = false;
                op.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:7060",
                    ValidAudience = "http://localhost:5455",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ASDF###@%#$Tf%$65464213sdFG353234324dfgadf"))
                };
                op.Events = new JwtBearerEvents
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
            builder.Services.AddAuthentication().AddGoogle(op =>
            {
                op.ClientId = builder.Configuration.GetSection("ClientId").ToString();
                op.ClientSecret = builder.Configuration.GetSection("Client_Secret").ToString();
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("public", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
                //op.AddPolicy("subscription", policy =>
                //{
                //    policy.WithOrigins("127.0.0.1").WithHeaders("token", "role").WithMethods("get");
                //});
            });
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRolesAsync(roleManager);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseCors("public");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");
            app.MapHub<NotificationHub>("/notificationhub");

            app.Run();
        }
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Enum.GetNames(typeof(RoleEnum)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
