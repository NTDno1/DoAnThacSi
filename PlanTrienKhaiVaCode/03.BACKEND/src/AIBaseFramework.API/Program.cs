// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Program.cs - Main Entry Point
// ============================================================

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using Serilog.Events;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.RAG;
using AIBaseFramework.AI.Search;
using AIBaseFramework.AI.Agent;
using AIBaseFramework.AI.Tools;
using AIBaseFramework.AI.SQL;
using AIBaseFramework.AI.Voice;
using AIBaseFramework.AI.MCP;
using AIBaseFramework.AI.Providers.Ollama;
using AIBaseFramework.AI.Providers.OpenAI;
using AIBaseFramework.Infrastructure.Data;
using AIBaseFramework.Infrastructure.AI;
using AIBaseFramework.Infrastructure.Cache;
using AIBaseFramework.Infrastructure.Storage;
using AIBaseFramework.API.Services.Interfaces;
using AIBaseFramework.API.Services.Implementations;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// LOGGING
// ============================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================================
// CONFIGURATION
// ============================================================

var configPath = Path.Combine(builder.Environment.ContentRootPath, "config.json");
AIPlatformConfig config;
if (File.Exists(configPath))
{
    config = System.Text.Json.JsonSerializer.Deserialize<AIPlatformConfig>(
        await File.ReadAllTextAsync(configPath)) ?? new AIPlatformConfig();
}
else
{
    config = new AIPlatformConfig();
}

builder.Services.AddSingleton(config);

// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<AIDbContext>(options =>
{
    options.UseNpgsql(config.Database.ConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
        npgsqlOptions.CommandTimeout(30);
    });
});

builder.Services.AddDbContextFactory<AIDbContext>(options =>
{
    options.UseNpgsql(config.Database.ConnectionString);
});

// ============================================================
// CACHE (Redis)
// ============================================================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = config.Redis.ConnectionString;
    options.InstanceName = "AIBF:";
});

builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// ============================================================
// AI PROVIDERS
// ============================================================

// Ollama Provider (Local LLM)
builder.Services.AddHttpClient<OllamaProvider>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(config.Ollama.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(config.Ollama.TimeoutSeconds);
    });

builder.Services.AddSingleton<OllamaProviderConfig>(sp => new OllamaProviderConfig
{
    BaseUrl = config.Ollama.BaseUrl,
    DefaultChatModel = config.Ollama.DefaultChatModel,
    DefaultEmbeddingModel = config.Ollama.DefaultEmbeddingModel,
    TimeoutSeconds = config.Ollama.TimeoutSeconds
});

builder.Services.AddSingleton<ILLMProvider, OllamaProvider>();
builder.Services.AddSingleton<IEmbeddingProvider, OllamaProvider>();

// OpenAI Provider (Cloud)
if (!string.IsNullOrEmpty(config.OpenAI.ApiKey))
{
    builder.Services.AddHttpClient<OpenAIProvider>()
        .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1");
            client.Timeout = TimeSpan.FromSeconds(config.OpenAI.TimeoutSeconds);
        });

    builder.Services.AddSingleton<OpenAIProviderConfig>(sp => new OpenAIProviderConfig
    {
        ApiKey = config.OpenAI.ApiKey,
        OrganizationId = config.OpenAI.OrganizationId,
        DefaultChatModel = config.OpenAI.DefaultChatModel,
        DefaultEmbeddingModel = config.OpenAI.DefaultEmbeddingModel,
        TimeoutSeconds = config.OpenAI.TimeoutSeconds
    });

    builder.Services.AddSingleton<ILLMProvider, OpenAIProvider>();
    builder.Services.AddSingleton<IEmbeddingProvider, OpenAIProvider>();
}

// AI Gateway
builder.Services.AddSingleton<AIGatewayConfig>(sp => new AIGatewayConfig
{
    DefaultRoutingStrategy = config.AI.DefaultRoutingStrategy,
    FallbackProviderOrder = config.AI.FallbackProviderOrder,
    EnableCostTracking = true,
    EnableLatencyTracking = true
});

builder.Services.AddSingleton<IAI Gateway, AIGateway>();

// ============================================================
// AI ENGINE CORE
// ============================================================

// Vector Store
builder.Services.AddSingleton<IVectorStore, PostgreSQLVectorStore>();

// RAG Pipeline
builder.Services.AddSingleton<RAGPipelineConfig>(sp => new RAGPipelineConfig
{
    ModelId = config.RAG.DefaultModel,
    Temperature = config.RAG.Temperature,
    MaxTokens = config.RAG.MaxTokens,
    MaxContextChunks = config.RAG.MaxContextChunks,
    MinRelevanceScore = config.RAG.MinRelevanceScore,
    EnableHybridSearch = config.RAG.EnableHybridSearch
});

builder.Services.AddSingleton<IRAGPipeline, RAGPipeline>();

// Search Engine
builder.Services.AddSingleton<SearchEngineConfig>(sp => new SearchEngineConfig
{
    EnableHybridSearch = config.Search.EnableHybridSearch,
    VectorWeight = config.Search.VectorWeight,
    KeywordWeight = config.Search.KeywordWeight
});

builder.Services.AddSingleton<ISearchEngine, SearchEngine>();
builder.Services.AddSingleton<ISearchRepository, PostgreSQLSearchRepository>();

// SQL Engine
builder.Services.AddSingleton<SQLEngineConfig>(sp => new SQLEngineConfig
{
    AllowDML = false,
    MaxResultRows = 100
});

builder.Services.AddSingleton<ISQLEngine, RAGSQLEngine>();
builder.Services.AddSingleton<ISQLSchemaProvider, DefaultSQLSchemaProvider>();

// ============================================================
// AGENT FRAMEWORK
// ============================================================

builder.Services.AddSingleton<AgentConfig>(sp => new AgentConfig
{
    MaxSteps = config.Agent.MaxSteps,
    MaxIterations = config.Agent.MaxIterations,
    SessionTimeoutMinutes = config.Agent.SessionTimeoutMinutes,
    EnableMemory = config.Agent.EnableMemory
});

builder.Services.AddSingleton<IToolRegistry, ToolRegistry>();
builder.Services.AddSingleton<IAPIConnectorRegistry, APIConnectorRegistry>();

// Built-in Tools
builder.Services.AddSingleton<IAITool, SearchTool>();
builder.Services.AddSingleton<IAITool, RAGQueryTool>();
builder.Services.AddSingleton<IAITool, LLMResponseTool>();

// Agent Memory
builder.Services.AddSingleton<IAgentMemoryService, AgentMemoryService>();

// Session Store
builder.Services.AddSingleton<ISessionStore, RedisSessionStore>();

// Safety Guard
builder.Services.AddSingleton<SafetyGuardConfig>(sp => new SafetyGuardConfig
{
    EnablePromptInjectionDetection = true,
    EnableSQLValidation = true,
    EnableContentFiltering = true
});

builder.Services.AddSingleton<IAgentSafetyGuard, AgentSafetyGuard>();

// Agent Orchestrator
builder.Services.AddSingleton<IAgentOrchestrator, AgentOrchestrator>();

// ============================================================
// VOICE AI
// ============================================================

builder.Services.AddSingleton<VoiceConfig>(sp => new VoiceConfig
{
    STTProvider = config.Voice.STTProvider,
    TTSProvider = config.Voice.TTSProvider,
    Language = config.Voice.Language
});

// STT Providers
builder.Services.AddHttpClient<OllamaWhisperSTTProvider>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(config.Ollama.BaseUrl);
    });

builder.Services.AddSingleton<ISTTProvider, OllamaWhisperSTTProvider>();

// TTS Providers
builder.Services.AddSingleton<ITTSProvider, LocalTTSProvider>();

// Voice Service
builder.Services.AddSingleton<IVoiceService, VoiceService>();

// ============================================================
// MCP SERVER
// ============================================================

builder.Services.AddSingleton<IMCPServer, MCPServer>();
builder.Services.AddSingleton<IMCPClient, MCPClient>();

// ============================================================
// AUTHENTICATION
// ============================================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config.Auth.JwtIssuer,
            ValidAudience = config.Auth.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config.Auth.JwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ============================================================
// SWAGGER
// ============================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Base Framework API",
        Version = "v1",
        Description = "Enterprise AI Platform API - Semantic Search, RAG, Agentic AI"
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

// ============================================================
// CONTROLLERS
// ============================================================

builder.Services.AddControllers();

// ============================================================
// BACKGROUND SERVICES
// ============================================================

builder.Services.AddHostedService<OllamaHealthCheckService>();

// ============================================================
// APPLICATION SERVICES
// ============================================================

// MinIO Service
builder.Services.AddSingleton<MinioClient>(sp =>
{
    var config = sp.GetRequiredService<AIPlatformConfig>();
    return new MinioClient()
        .WithEndpoint(config.MinIO.Endpoint)
        .WithCredentials(config.MinIO.AccessKey, config.MinIO.SecretKey)
        .WithSSL(config.MinIO.UseSSL)
        .Build();
});

builder.Services.AddSingleton<IMinIOService, MinIOService>();

// Application Services
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ============================================================
// BUILD APP
// ============================================================

var app = builder.Build();

// ============================================================
// MIDDLEWARE
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Base Framework API v1");
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// ============================================================
// INITIALIZATION
// ============================================================

// Register tools on startup
using (var scope = app.Services.CreateScope())
{
    var toolRegistry = scope.ServiceProvider.GetRequiredService<IToolRegistry>();
    var tools = scope.ServiceProvider.GetRequiredService<IEnumerable<IAITool>>();
    toolRegistry.RegisterTools(tools);

    Log.Information("Registered {Count} AI tools", tools.Count());
}

// Start MCP server
var mcpServer = app.Services.GetRequiredService<IMCPServer>();
await mcpServer.StartAsync();

Log.Information("AI Base Framework started on port {Port}", config.App.Port);

app.Run($"http://0.0.0.0:{config.App.Port}");

// ============================================================
// BACKGROUND SERVICES
// ============================================================

public class OllamaHealthCheckService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OllamaHealthCheckService> _logger;

    public OllamaHealthCheckService(IServiceProvider serviceProvider, ILogger<OllamaHealthCheckService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var ollama = scope.ServiceProvider.GetService<ILLMProvider>();

                if (ollama != null)
                {
                    var health = await ollama.GetHealthStatusAsync(stoppingToken);
                    if (!health.IsHealthy)
                    {
                        _logger.LogWarning("Ollama health check failed: {Message}", health.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ollama health check error");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

// ============================================================
// CONFIG CLASSES
// ============================================================

public class AIPlatformConfig
{
    public AppConfig App { get; set; } = new();
    public DatabaseConfig Database { get; set; } = new();
    public RedisConfig Redis { get; set; } = new();
    public OllamaConfig Ollama { get; set; } = new();
    public OpenAIConfig OpenAI { get; set; } = new();
    public AIConfig AI { get; set; } = new();
    public RAGConfig RAG { get; set; } = new();
    public SearchConfig Search { get; set; } = new();
    public AgentConfig Agent { get; set; } = new();
    public VoiceConfig2 Voice { get; set; } = new();
    public AuthConfig Auth { get; set; } = new();
    public MinIOConfig MinIO { get; set; } = new();
}

public class AppConfig
{
    public int Port { get; set; } = 5000;
    public string Environment { get; set; } = "Development";
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = "Host=localhost;Database=aibaseframework;Username=aibfuser;Password=aibfpass123";
}

public class RedisConfig
{
    public string ConnectionString { get; set; } = "localhost:6379,password=redis123";
}

public class OllamaConfig
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultChatModel { get; set; } = "llama3.2:3b";
    public string DefaultEmbeddingModel { get; set; } = "nomic-embed-text";
    public int TimeoutSeconds { get; set; } = 120;
}

public class OpenAIConfig
{
    public string ApiKey { get; set; } = "";
    public string OrganizationId { get; set; } = "";
    public string DefaultChatModel { get; set; } = "gpt-4o-mini";
    public string DefaultEmbeddingModel { get; set; } = "text-embedding-3-small";
    public int TimeoutSeconds { get; set; } = 60;
}

public class AIConfig
{
    public RoutingStrategy DefaultRoutingStrategy { get; set; } = RoutingStrategy.Fallback;
    public List<string> FallbackProviderOrder { get; set; } = new() { "ollama", "openai" };
}

public class RAGConfig
{
    public string DefaultModel { get; set; } = "llama3.2:3b";
    public double Temperature { get; set; } = 0.3;
    public int MaxTokens { get; set; } = 2048;
    public int MaxContextChunks { get; set; } = 10;
    public double MinRelevanceScore { get; set; } = 0.5;
    public bool EnableHybridSearch { get; set; } = true;
}

public class SearchConfig
{
    public bool EnableHybridSearch { get; set; } = true;
    public double VectorWeight { get; set; } = 0.7;
    public double KeywordWeight { get; set; } = 0.3;
}

public class AgentConfig
{
    public int MaxSteps { get; set; } = 10;
    public int MaxIterations { get; set; } = 5;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public bool EnableMemory { get; set; } = true;
}

public class VoiceConfig2
{
    public string STTProvider { get; set; } = "whisper";
    public string TTSProvider { get; set; } = "local";
    public string Language { get; set; } = "vi";
}

public class AuthConfig
{
    public string JwtSecret { get; set; } = "your-super-secret-key-minimum-32-characters";
    public string JwtIssuer { get; set; } = "AIBaseFramework";
    public string JwtAudience { get; set; } = "AIBaseFramework";
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}

public class MinIOConfig
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public string BucketName { get; set; } = "documents";
    public bool UseSSL { get; set; } = false;
}
