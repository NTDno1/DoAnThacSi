// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: MCP Server Integration
// ============================================================

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Tools;

namespace AIBaseFramework.AI.MCP;

// ============================================================
// MCP PROTOCOL TYPES
// ============================================================

/// <summary>
/// MCP message types
/// </summary>
public enum MCPMessageType
{
    JsonrpcRequest,
    JsonrpcResponse,
    JsonrpcError,
    JsonrpcNotification
}

/// <summary>
/// MCP request
/// </summary>
public class MCPRequest
{
    public string Jsonrpc { get; set; } = "2.0";
    public string? Method { get; set; }
    public JsonElement? Params { get; set; }
    public string? Id { get; set; }
}

/// <summary>
/// MCP response
/// </summary>
public class MCPResponse
{
    public string Jsonrpc { get; set; } = "2.0";
    public object? Result { get; set; }
    public MCPError? Error { get; set; }
    public string? Id { get; set; }
}

/// <summary>
/// MCP error
/// </summary>
public class MCPError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

/// <summary>
/// MCP tool
/// </summary>
public class MCPTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, MCPToolParameter> InputSchema { get; set; } = new();
}

/// <summary>
/// MCP tool parameter
/// </summary>
public class MCPToolParameter
{
    public string Type { get; set; } = "string";
    public string? Description { get; set; }
    public bool Required { get; set; }
    public object? Default { get; set; }
}

// ============================================================
// MCP SERVER
// ============================================================

/// <summary>
/// MCP Server - Provides tools to AI agents via MCP protocol
/// </summary>
public interface IMCPServer
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    void RegisterTool(MCPTool tool, Func<Dictionary<string, object>, CancellationToken, Task<object>> handler);
    Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken);
}

/// <summary>
/// MCP Server implementation
/// </summary>
public class MCPServer : IMCPServer
{
    private readonly IToolRegistry _toolRegistry;
    private readonly ILogger<MCPServer> _logger;
    private readonly Dictionary<string, (MCPTool Tool, Func<Dictionary<string, object>, CancellationToken, Task<object>> Handler)> _registeredTools = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isRunning;
    
    public MCPServer(IToolRegistry toolRegistry, ILogger<MCPServer> logger)
    {
        _toolRegistry = toolRegistry;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
    
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = true;
        _logger.LogInformation("MCP Server started");
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = false;
        _logger.LogInformation("MCP Server stopped");
        return Task.CompletedTask;
    }
    
    public void RegisterTool(
        MCPTool tool, 
        Func<Dictionary<string, object>, CancellationToken, Task<object>> handler)
    {
        _registeredTools[tool.Name] = (tool, handler);
        _logger.LogInformation("MCP Tool registered: {Name}", tool.Name);
    }
    
    /// <summary>
    /// Handle WebSocket connection
    /// </summary>
    public async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MCP client connected");
        
        try
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var message = await ReceiveMessageAsync(webSocket, cancellationToken);
                if (message == null) break;
                
                var response = await ProcessMessageAsync(message, cancellationToken);
                
                if (response != null)
                {
                    await SendMessageAsync(webSocket, response, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP WebSocket error");
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
            _logger.LogInformation("MCP client disconnected");
        }
    }
    
    private async Task<MCPRequest?> ReceiveMessageAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];
        var message = new StringBuilder();
        
        while (true)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }
            
            message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            
            if (result.EndOfMessage)
            {
                break;
            }
        }
        
        try
        {
            return JsonSerializer.Deserialize<MCPRequest>(message.ToString(), _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse MCP message");
            return null;
        }
    }
    
    private async Task<MCPResponse?> ProcessMessageAsync(MCPRequest request, CancellationToken cancellationToken)
    {
        if (request.Method == null)
        {
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError { Code = -32600, Message = "Invalid Request" }
            };
        }
        
        try
        {
            return request.Method switch
            {
                "initialize" => HandleInitialize(request),
                "tools/list" => HandleListTools(request),
                "tools/call" => await HandleCallToolAsync(request, cancellationToken),
                "ping" => new MCPResponse { Id = request.Id, Result = new { status = "pong" } },
                _ => new MCPResponse
                {
                    Id = request.Id,
                    Error = new MCPError { Code = -32601, Message = $"Method not found: {request.Method}" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request: {Method}", request.Method);
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError { Code = -32603, Message = $"Internal error: {ex.Message}" }
            };
        }
    }
    
    private MCPResponse HandleInitialize(MCPRequest request)
    {
        return new MCPResponse
        {
            Id = request.Id,
            Result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { },
                    resources = new { }
                },
                serverInfo = new
                {
                    name = "AIBaseFramework-MCP",
                    version = "1.0.0"
                }
            }
        };
    }
    
    private MCPResponse HandleListTools(MCPRequest request)
    {
        var tools = _registeredTools.Values.Select(t => t.Tool).ToList();
        
        // Also get tools from tool registry
        var registryTools = _toolRegistry.GetToolsMetadata().Select(m => new MCPTool
        {
            Name = m.Name,
            Description = m.Description,
            InputSchema = new Dictionary<string, MCPToolParameter>
            {
                ["query"] = new MCPToolParameter { Type = "string", Description = "Query parameter", Required = true }
            }
        }).ToList();
        
        tools.AddRange(registryTools);
        
        return new MCPResponse
        {
            Id = request.Id,
            Result = new { tools }
        };
    }
    
    private async Task<MCPResponse> HandleCallToolAsync(MCPRequest request, CancellationToken cancellationToken)
    {
        if (request.Params == null)
        {
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError { Code = -32602, Message = "Invalid params" }
            };
        }
        
        var toolName = request.Params.Value.GetProperty("name").GetString();
        var arguments = new Dictionary<string, object>();
        
        if (request.Params.Value.TryGetProperty("arguments", out var argsElement))
        {
            arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(argsElement.GetRawText(), _jsonOptions) ?? new();
        }
        
        if (string.IsNullOrEmpty(toolName))
        {
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError { Code = -32602, Message = "Tool name is required" }
            };
        }
        
        // Check registered tools first
        if (_registeredTools.TryGetValue(toolName, out var registeredTool))
        {
            var result = await registeredTool.Handler(arguments, cancellationToken);
            return new MCPResponse
            {
                Id = request.Id,
                Result = new
                {
                    content = new[]
                    {
                        new { type = "text", text = result?.ToString() ?? "" }
                    }
                }
            };
        }
        
        // Fall back to tool registry
        var tool = _toolRegistry.GetTool(toolName);
        if (tool == null)
        {
            return new MCPResponse
            {
                Id = request.Id,
                Error = new MCPError { Code = -32602, Message = $"Tool not found: {toolName}" }
            };
        }
        
        // Create agent session for tool execution
        var session = new AIBaseFramework.AI.Agent.AgentSession();
        var result2 = await tool.ExecuteAsync(arguments, session, cancellationToken);
        
        return new MCPResponse
        {
            Id = request.Id,
            Result = new
            {
                content = new[]
                {
                    new { type = "text", text = result2 }
                }
            }
        };
    }
    
    private async Task SendMessageAsync(WebSocket webSocket, MCPResponse response, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }
}

// ============================================================
// MCP CLIENT
// ============================================================

/// <summary>
/// MCP Client - Connect to external MCP servers
/// </summary>
public interface IMCPClient
{
    Task ConnectAsync(string serverUrl, CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    Task<List<MCPTool>> GetToolsAsync(CancellationToken cancellationToken = default);
    Task<object> CallToolAsync(string toolName, Dictionary<string, object> arguments, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}

public class MCPClient : IMCPClient
{
    private ClientWebSocket? _webSocket;
    private readonly ILogger<MCPClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private List<MCPTool>? _cachedTools;
    
    public bool IsConnected => _webSocket?.State == WebSocketState.Open;
    
    public MCPClient(ILogger<MCPClient> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    public async Task ConnectAsync(string serverUrl, CancellationToken cancellationToken = default)
    {
        _webSocket = new ClientWebSocket();
        
        var uri = new Uri(serverUrl.StartsWith("ws") ? serverUrl : $"ws://{serverUrl}");
        await _webSocket.ConnectAsync(uri, cancellationToken);
        
        // Initialize
        var initRequest = new MCPRequest
        {
            Method = "initialize",
            Params = JsonSerializer.Deserialize<JsonElement>("{}"),
            Id = "1"
        };
        
        await SendRequestAsync(initRequest, cancellationToken);
        
        // List tools
        await GetToolsAsync(cancellationToken);
        
        _logger.LogInformation("Connected to MCP server: {Url}", serverUrl);
    }
    
    public async Task DisconnectAsync()
    {
        if (_webSocket != null)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket = null;
        }
    }
    
    public async Task<List<MCPTool>> GetToolsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedTools != null) return _cachedTools;
        
        var request = new MCPRequest { Method = "tools/list", Id = "2" };
        var response = await SendRequestAsync(request, cancellationToken);
        
        _cachedTools = response?.Result != null 
            ? JsonSerializer.Deserialize<List<MCPTool>>(response.Result.ToString() ?? "[]", _jsonOptions) ?? new()
            : new List<MCPTool>();
        
        return _cachedTools;
    }
    
    public async Task<object> CallToolAsync(
        string toolName, 
        Dictionary<string, object> arguments, 
        CancellationToken cancellationToken = default)
    {
        var request = new MCPRequest
        {
            Method = "tools/call",
            Params = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
            {
                name = toolName,
                arguments
            }, _jsonOptions)),
            Id = Guid.NewGuid().ToString()
        };
        
        var response = await SendRequestAsync(request, cancellationToken);
        return response?.Result ?? new object();
    }
    
    private async Task<MCPResponse?> SendRequestAsync(MCPRequest request, CancellationToken cancellationToken)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("Not connected to MCP server");
        }
        
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
        
        // Wait for response
        var buffer = new byte[8192];
        var message = new StringBuilder();
        
        while (true)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            
            if (result.EndOfMessage) break;
        }
        
        return JsonSerializer.Deserialize<MCPResponse>(message.ToString(), _jsonOptions);
    }
}
