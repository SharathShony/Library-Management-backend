using Libraray.Api.Context;
using Libraray.Api.Repositories;
using Libraray.Api.Repositories.Interfaces;
using Libraray.Api.Services;
using Libraray.Api.Services.Interfaces;
using Library_backend.Repositories;
using Library_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Libraray.Api.Helpers.StoredProcedures;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token. Example: 'Bearer eyJhbGc...'"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 🔥 HELPER: Convert Render/Supabase postgres:// URL to Npgsql connection string format
static string ConvertDatabaseUrl(string databaseUrl)
{
    if (string.IsNullOrEmpty(databaseUrl) || !databaseUrl.StartsWith("postgres://"))
        return databaseUrl; // Already in correct format or null

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    return $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

// 🔥 UPDATED: Support environment variables for production (Render/Supabase)
var rawConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not configured");

var connectionString = ConvertDatabaseUrl(rawConnectionString);

// 🔥 CHANGED: UseSqlServer → UseNpgsql for PostgreSQL/Supabase
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(connectionString));

// ? Register Connection Factory for Stored Procedures (PostgreSQL)
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new NpgsqlConnectionFactory(connectionString);
});

// Then register repositories and services that depend on it
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
           .AllowAnyHeader()
       .AllowAnyMethod();
});
});

// 🔥 UPDATED: JWT Configuration with environment variable support
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
?? builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key not configured");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
    ?? builder.Configuration["Jwt:Issuer"] 
  ?? throw new InvalidOperationException("JWT Issuer not configured");

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
    ?? builder.Configuration["Jwt:Audience"] 
    ?? throw new InvalidOperationException("JWT Audience not configured");

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
    {
   ValidateIssuer = true,
  ValidateAudience = true,
 ValidateLifetime = true,
  ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
  ValidAudience = jwtAudience,
   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
// 🔥 UPDATED: Enable Swagger in production for Render (you can disable later)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Comment out or remove HTTPS redirection for local development
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
