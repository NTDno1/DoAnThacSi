// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Voice AI - Speech to Text & Text to Speech
// ============================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Voice;

// ============================================================
// VOICE TYPES
// ============================================================

/// <summary>
/// Voice configuration
/// </summary>
public class VoiceConfig
{
    public string STTProvider { get; set; } = "whisper";
    public string TTSProvider { get; set; } = "local";
    public string STTModel { get; set; } = "base";
    public string TTSVoice { get; set; } = "default";
    public string Language { get; set; } = "vi";
    public double STTConfidenceThreshold { get; set; } = 0.7;
    public int MaxAudioDurationSeconds { get; set; } = 60;
}

/// <summary>
/// STT Request
/// </summary>
public class STTRequest
{
    public byte[] AudioData { get; set; } = Array.Empty<byte>();
    public string? AudioUrl { get; set; }
    public string Language { get; set; } = "vi";
    public string Format { get; set; } = "wav";
    public int SampleRate { get; set; } = 16000;
    public bool GetTimestamps { get; set; } = true;
}

/// <summary>
/// STT Response
/// </summary>
public class STTResponse
{
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<WordTimestamp> WordTimestamps { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public string? SpeakerDiarization { get; set; }
}

/// <summary>
/// Word timestamp
/// </summary>
public class WordTimestamp
{
    public string Word { get; set; } = string.Empty;
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// TTS Request
/// </summary>
public class TTSRequest
{
    public string Text { get; set; } = string.Empty;
    public string Voice { get; set; } = "default";
    public string Language { get; set; } = "vi";
    public double Speed { get; set; } = 1.0;
    public double Pitch { get; set; } = 1.0;
    public string Format { get; set; } = "mp3";
    public int SampleRate { get; set; } = 22050;
}

/// <summary>
/// TTS Response
/// </summary>
public class TTSResponse
{
    public byte[] AudioData { get; set; } = Array.Empty<byte>();
    public string Format { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? AudioUrl { get; set; }
}

// ============================================================
// VOICE SERVICE
// ============================================================

/// <summary>
/// Voice Service - Combined STT and TTS
/// </summary>
public interface IVoiceService
{
    Task<STTResponse> TranscribeAsync(STTRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<STTResponse> TranscribeStreamAsync(STTRequest request, CancellationToken cancellationToken = default);
    Task<TTSResponse> SynthesizeAsync(TTSRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<byte[]> SynthesizeStreamAsync(TTSRequest request, CancellationToken cancellationToken = default);
    Task<VoiceIntent> RecognizeIntentAsync(byte[] audioData, CancellationToken cancellationToken = default);
}

/// <summary>
/// Voice intent
/// </summary>
public class VoiceIntent
{
    public string Text { get; set; } = string.Empty;
    public string? Intent { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public double Confidence { get; set; }
}

/// <summary>
/// Voice Service implementation
/// </summary>
public class VoiceService : IVoiceService
{
    private readonly IAI Gateway _aiGateway;
    private readonly IEnumerable<ISTTProvider> _sttProviders;
    private readonly IEnumerable<ITTSProvider> _ttsProviders;
    private readonly ILogger<VoiceService> _logger;
    private readonly VoiceConfig _config;
    
    public VoiceService(
        IAI Gateway aiGateway,
        IEnumerable<ISTTProvider> sttProviders,
        IEnumerable<ITTSProvider> ttsProviders,
        ILogger<VoiceService> logger,
        VoiceConfig config)
    {
        _aiGateway = aiGateway;
        _sttProviders = sttProviders;
        _ttsProviders = ttsProviders;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Transcribe audio to text
    /// </summary>
    public async Task<STTResponse> TranscribeAsync(STTRequest request, CancellationToken cancellationToken = default)
    {
        var provider = _sttProviders.FirstOrDefault(p => p.ProviderId == _config.STTProvider);
        
        if (provider == null)
        {
            // Use AI for transcription
            return await TranscribeWithAIAsync(request, cancellationToken);
        }
        
        return await provider.TranscribeAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Stream transcription
    /// </summary>
    public async IAsyncEnumerable<STTResponse> TranscribeStreamAsync(
        STTRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var provider = _sttProviders.FirstOrDefault(p => p.ProviderId == _config.STTProvider);
        
        if (provider != null)
        {
            await foreach (var result in provider.TranscribeStreamAsync(request, cancellationToken))
            {
                yield return result;
            }
        }
    }
    
    /// <summary>
    /// Synthesize text to speech
    /// </summary>
    public async Task<TTSResponse> SynthesizeAsync(TTSRequest request, CancellationToken cancellationToken = default)
    {
        var provider = _ttsProviders.FirstOrDefault(p => p.ProviderId == _config.TTSProvider);
        
        if (provider == null)
        {
            throw new InvalidOperationException($"TTS provider not found: {_config.TTSProvider}");
        }
        
        return await provider.SynthesizeAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Stream synthesis
    /// </summary>
    public async IAsyncEnumerable<byte[]> SynthesizeStreamAsync(
        TTSRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var provider = _ttsProviders.FirstOrDefault(p => p.ProviderId == _config.TTSProvider);
        
        if (provider != null)
        {
            await foreach (var chunk in provider.SynthesizeStreamAsync(request, cancellationToken))
            {
                yield return chunk;
            }
        }
    }
    
    /// <summary>
    /// Recognize voice intent
    /// </summary>
    public async Task<VoiceIntent> RecognizeIntentAsync(byte[] audioData, CancellationToken cancellationToken = default)
    {
        // Step 1: Transcribe
        var sttRequest = new STTRequest
        {
            AudioData = audioData,
            Language = _config.Language
        };
        
        var transcription = await TranscribeAsync(sttRequest, cancellationToken);
        
        // Step 2: Classify intent
        var intentResponse = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = @"You are an intent classifier for voice commands. 
Classify the user's voice input into one of these intents:
- search: Search for documents
- create: Create new items
- update: Update existing items
- delete: Delete items
- chat: Casual conversation
- control: System control (play, pause, etc.)

Return JSON:
{
  ""intent"": ""intent_name"",
  ""confidence"": 0.0-1.0,
  ""parameters"": { ""param"": ""value"" }
}" },
                new() { Role = MessageRole.User, Content = transcription.Text }
            },
            Options = new AIRequestOptions { Temperature = 0.3, MaxTokens = 200 }
        }, cancellationToken);
        
        VoiceIntent result;
        try
        {
            result = System.Text.Json.JsonSerializer.Deserialize<VoiceIntent>(intentResponse.Content) ?? new VoiceIntent();
        }
        catch
        {
            result = new VoiceIntent();
        }
        
        result.Text = transcription.Text;
        result.Confidence *= transcription.Confidence;
        
        return result;
    }
    
    /// <summary>
    /// Transcribe using AI (fallback)
    /// </summary>
    private async Task<STTResponse> TranscribeWithAIAsync(STTRequest request, CancellationToken cancellationToken)
    {
        // Use Whisper API if available
        var whisperProvider = _sttProviders.FirstOrDefault(p => p.ProviderId == "whisper");
        
        if (whisperProvider != null)
        {
            return await whisperProvider.TranscribeAsync(request, cancellationToken);
        }
        
        // Fallback: Return empty response
        return new STTResponse
        {
            Text = "",
            Confidence = 0,
            Language = request.Language
        };
    }
}

// ============================================================
// STT PROVIDER
// ============================================================

public interface ISTTProvider
{
    string ProviderId { get; }
    Task<STTResponse> TranscribeAsync(STTRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<STTResponse> TranscribeStreamAsync(STTRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Ollama Whisper STT Provider (using local Whisper model)
/// </summary>
public class OllamaWhisperSTTProvider : ISTTProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaWhisperSTTProvider> _logger;
    private readonly string _model;
    
    public string ProviderId => "whisper";
    
    public OllamaWhisperSTTProvider(HttpClient httpClient, ILogger<OllamaWhisperSTTProvider> logger, string model = "whisper")
    {
        _httpClient = httpClient;
        _logger = logger;
        _model = model;
        
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
    }
    
    public async Task<STTResponse> TranscribeAsync(STTRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var content = new MultipartFormDataContent();
        
        if (request.AudioData.Length > 0)
        {
            var byteContent = new ByteArrayContent(request.AudioData);
            content.Add(byteContent, "file", "audio.wav");
        }
        
        content.Add(new StringContent(_model), "model");
        content.Add(new StringContent("true"), "stream");
        content.Add(new StringContent(request.Language), "language");
        
        var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<WhisperResponse>(cancellationToken: cancellationToken);
        
        stopwatch.Stop();
        
        return new STTResponse
        {
            Text = result?.Response ?? string.Empty,
            Language = request.Language,
            Confidence = 0.9,
            Duration = stopwatch.Elapsed
        };
    }
    
    public async IAsyncEnumerable<STTResponse> TranscribeStreamAsync(
        STTRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Stream transcription
        await foreach (var _ in TranscribeAsync(request, cancellationToken).ConfigureAwait(false))
        {
            yield break;
        }
    }
    
    private class WhisperResponse
    {
        public string? Response { get; set; }
    }
}

/// <summary>
/// OpenAI Whisper STT Provider
/// </summary>
public class OpenAIWhisperSTTProvider : ISTTProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIWhisperSTTProvider> _logger;
    private readonly string _apiKey;
    
    public string ProviderId => "openai-whisper";
    
    public OpenAIWhisperSTTProvider(HttpClient httpClient, ILogger<OpenAIWhisperSTTProvider> logger, string apiKey)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = apiKey;
        
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    public async Task<STTResponse> TranscribeAsync(STTRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var content = new MultipartFormDataContent();
        
        if (request.AudioData.Length > 0)
        {
            var byteContent = new ByteArrayContent(request.AudioData);
            content.Add(byteContent, "file", "audio.wav");
            content.Add(new StringContent(request.Language), "language");
            content.Add(new StringContent("json"), "response_format");
        }
        
        var response = await _httpClient.PostAsync("/audio/transcriptions", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<OpenAIWhisperResponse>(cancellationToken: cancellationToken);
        
        stopwatch.Stop();
        
        return new STTResponse
        {
            Text = result?.Text ?? string.Empty,
            Language = request.Language,
            Confidence = 0.95,
            Duration = stopwatch.Elapsed,
            WordTimestamps = result?.Words?.Select(w => new WordTimestamp
            {
                Word = w.Word,
                Start = TimeSpan.FromSeconds(w.Start),
                End = TimeSpan.FromSeconds(w.End),
                Confidence = w.Probability
            }).ToList() ?? new List<WordTimestamp>()
        };
    }
    
    public async IAsyncEnumerable<STTResponse> TranscribeStreamAsync(
        STTRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await TranscribeAsync(request, cancellationToken);
        yield return result;
    }
    
    private class OpenAIWhisperResponse
    {
        public string? Text { get; set; }
        public List<WordInfo>? Words { get; set; }
    }
    
    private class WordInfo
    {
        public string Word { get; set; } = string.Empty;
        public double Start { get; set; }
        public double End { get; set; }
        public double Probability { get; set; }
    }
}

// ============================================================
// TTS PROVIDER
// ============================================================

public interface ITTSProvider
{
    string ProviderId { get; }
    Task<TTSResponse> SynthesizeAsync(TTSRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<byte[]> SynthesizeStreamAsync(TTSRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Local TTS Provider (using Piper or Coqui)
/// </summary>
public class LocalTTSProvider : ITTSProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocalTTSProvider> _logger;
    private readonly string _model;
    
    public string ProviderId => "local";
    
    public LocalTTSProvider(HttpClient httpClient, ILogger<LocalTTSProvider> logger, string model = "default")
    {
        _httpClient = httpClient;
        _logger = logger;
        _model = model;
    }
    
    public async Task<TTSResponse> SynthesizeAsync(TTSRequest request, CancellationToken cancellationToken = default)
    {
        // This would integrate with a local TTS service like Piper
        // For now, return a placeholder response
        
        _logger.LogWarning("Local TTS not fully implemented - would call Piper/Coqui API");
        
        return new TTSResponse
        {
            AudioData = Array.Empty<byte>(),
            Format = request.Format,
            Duration = TimeSpan.FromSeconds(request.Text.Length / 10.0)
        };
    }
    
    public async IAsyncEnumerable<byte[]> SynthesizeStreamAsync(
        TTSRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await SynthesizeAsync(request, cancellationToken);
        if (result.AudioData.Length > 0)
        {
            yield return result.AudioData;
        }
    }
}

/// <summary>
/// OpenAI TTS Provider
/// </summary>
public class OpenAITTSProvider : ITTSProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAITTSProvider> _logger;
    private readonly string _apiKey;
    
    public string ProviderId => "openai-tts";
    
    public OpenAITTSProvider(HttpClient httpClient, ILogger<OpenAITTSProvider> logger, string apiKey)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = apiKey;
        
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    public async Task<TTSResponse> SynthesizeAsync(TTSRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var formData = new Dictionary<string, string>
        {
            ["model"] = "tts-1",
            ["input"] = request.Text,
            ["voice"] = MapVoice(request.Voice),
            ["response_format"] = request.Format,
            ["speed"] = request.Speed.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };
        
        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync("/audio/speech", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var audioData = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        
        stopwatch.Stop();
        
        return new TTSResponse
        {
            AudioData = audioData,
            Format = request.Format,
            Duration = stopwatch.Elapsed
        };
    }
    
    public async IAsyncEnumerable<byte[]> SynthesizeStreamAsync(
        TTSRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await SynthesizeAsync(request, cancellationToken);
        
        // Stream in chunks
        const int chunkSize = 4096;
        for (int i = 0; i < result.AudioData.Length; i += chunkSize)
        {
            var chunk = new byte[Math.Min(chunkSize, result.AudioData.Length - i)];
            Array.Copy(result.AudioData, i, chunk, 0, chunk.Length);
            yield return chunk;
        }
    }
    
    private static string MapVoice(string voice)
    {
        return voice.ToLower() switch
        {
            "female" or "nữ" => "alloy",
            "male" or "nam" => "echo",
            _ => "nova"
        };
    }
}
