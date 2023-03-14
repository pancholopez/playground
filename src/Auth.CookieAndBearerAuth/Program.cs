using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Auth.CookieAndBearerAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllers(); // support api controllers

            builder.Services.AddEndpointsApiExplorer(); // support swagger
            builder.Services.AddSwaggerGen();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "JWT_OR_COOKIE";
                    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
                })
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = "CookiesAndTokens";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey("super-secret-key"u8.ToArray())
                    };
                })
                .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
                {
                    // runs on each request
                    options.ForwardDefaultSelector = context =>
                    {
                        string authorization = context.Request.Headers[HeaderNames.Authorization]!;
                        return !string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ")
                            ? "Bearer" // use bearer auth
                            : "Cookies"; // otherwise default to cookie auth
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => { policy.RequireClaim("Admin"); });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}