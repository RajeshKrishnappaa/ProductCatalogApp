
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdApp.Configuration;
using ProdApp.Data;
using AutoMapper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ProdApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProdDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ProdAppConnection")));

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme. Enter Bearer [space] add your token in the text input",
                    Name = "Authorization",
                    In=ParameterLocation.Header,
                    Scheme="Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Id ="Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "Bearer",
                            Name="Bearer",
                            In=ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtConfig:Key"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true
                };

            });
            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ProdDbContext>();
                if (!context.Users.Any())
                {
                    context.Users.AddRange(new Models.User
                    {
                        UserName = "Rajesh",
                        Email = "Rajesh@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rajesh123"),
                        Role = "Admin",
                        RegisteredAt = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
           
            app.MapControllers();

            app.Run();
        }
    }
}
