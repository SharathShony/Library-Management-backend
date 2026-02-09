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
using Npgsql;

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

// 🔥 Build connection string safely using separate env vars OR postgres:// URL
static string BuildConnectionString()
{
    // Option 1: Use separate environment variables (RECOMMENDED - handles special chars in password)
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    
    if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
    {
        var csBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = dbHost,
            Port = int.Parse(Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"),
            Database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "postgres",
            Username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres",
            Password = dbPassword,
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        
        Console.WriteLine($"✅ Built connection string from separate env vars");
        Console.WriteLine($"   Host: {csBuilder.Host}, Port: {csBuilder.Port}, Database: {csBuilder.Database}");
        
        return csBuilder.ConnectionString;
    }
    
    // Option 2: Use postgres:// URL format
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo;
        var colonIndex = userInfo.IndexOf(':');
        
        var csBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = uri.LocalPath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo.Substring(0, colonIndex)),
            Password = Uri.UnescapeDataString(userInfo.Substring(colonIndex + 1)),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        
        Console.WriteLine($"✅ Built connection string from DATABASE_URL");
        return csBuilder.ConnectionString;
    }
    
    // Option 3: Fallback to raw DATABASE_URL or appsettings
    return databaseUrl 
        ?? throw new InvalidOperationException("Database not configured. Set DB_HOST + DB_PASSWORD or DATABASE_URL");
}

// 🔥 UPDATED: Build connection string safely
var connectionString = BuildConnectionString();

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
