// Program.cs - Entry Point
// AIBaseFramework - Semantic Search + RAG Chatbot

using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using AIBaseFramework.API.Common.Middleware;
using AIBaseFramework.API.Common.Extensions;
using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.Data;
using AIBaseFramework.API.Infrastructure.AI;
using AIBaseFramework.API.Infrastructure.Cache;
using AIBaseFramework.API.Infrastructure.Storage;
using AIBaseFramework.API.Services.Implementations;
using AIBaseFramework.API.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5340")
    .CreateLogger();

try
{
    Log.Information("Starting AIBaseFramework API...");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // =============================================
    // DATABASE - PostgreSQL + pgvector
    // =============================================
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.CommandTimeout(30);
            });
    });

    // =============================================
    // AUTHENTICATION - JWT Bearer
    // =============================================
    var jwtSecret = builder.Configuration["JwtConfig:Secret"] 
        ?? throw new InvalidOperationException("JWT Secret not configured");
    var jwtIssuer = builder.Configuration["JwtConfig:Issuer"] ?? "AIBaseFrameworkAPI";
    var jwtAudience = builder.Configuration["JwtConfig:Audience"] ?? "AIBaseFrameworkClient";

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
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    // =============================================
    // MEDIATR - CQRS Pipeline
    // =============================================
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    });

    // =============================================
    // VALIDATION - FluentValidation
    // =============================================
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // =============================================
    // CORS
    // =============================================
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")
        .Get<string[]>() ?? new[] { "http://localhost:3000" };

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        });
    });

    // =============================================
    // SERVICES - Dependency Injection
    // =============================================

    // Infrastructure Services
    builder.Services.AddScoped<IOllamaService, OllamaService>();
    builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
    builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
    builder.Services.AddScoped<IMinIOService, MinIOService>();
    builder.Services.AddScoped<IRAGPipeline, RAGPipeline>();

    // Business Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IDocumentService, DocumentService>();
    builder.Services.AddScoped<ISearchService, SearchService>();
    builder.Services.AddScoped<IChatService, ChatService>();

    // =============================================
    // API DOCUMENTATION - Swagger
    // =============================================
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AIBaseFramework API",
            Version = "v1",
            Description = "AI Base Framework - Semantic Search + RAG Chatbot API - 100% Offline Local"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // =============================================
    // BACKGROUND JOBS - Hangfire
    // =============================================
    builder.Services.AddHangfire(config =>
    {
        config.UsePostgreSqlStorage(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddHangfireServer();

    // =============================================
    // API CONTROLLERS
    // =============================================
    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    // =============================================
    // BUILD APP
    // =============================================
    var app = builder.Build();

    // =============================================
    // PIPELINE - Middleware
    // =============================================

    // Global Exception Handler
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Swagger (Development)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIBaseFramework API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });

    app.MapControllers();
    app.MapHealthChecks("/health");

    // =============================================
    // STARTUP - Run migrations & Start
    // =============================================

    // Apply migrations automatically in Development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    Log.Information("AIBaseFramework API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
