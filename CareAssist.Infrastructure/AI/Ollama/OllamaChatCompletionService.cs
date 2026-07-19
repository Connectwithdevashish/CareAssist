using CareAssist.Application.Abstractions.AI;
using CareAssist.Contracts.AI;
using CareAssist.Infrastructure.AI.Ollama.Contracts;
using CareAssist.Infrastructure.AI.Ollama.Mapping;
using CareAssist.Infrastructure.Configuration.AI;
using CareAssist.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace CareAssist.Infrastructure.AI.Ollama;

public sealed class OllamaChatCompletionService : IChatCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _aiOptions;
    private readonly ILogger<OllamaChatCompletionService> _logger;
    private const string chatEndpoint = "/api/chat";

    public OllamaChatCompletionService(
        IOptions<AIOptions> aiOptions,
        HttpClient httpClient,
        ILogger<OllamaChatCompletionService> logger )
    {
        _aiOptions = aiOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ChatResponse> GenerateResponseAsync(IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default)
    {
        var request = BuildRequest(messages);

        HttpResponseMessage response = await SendRequestAsync(request, cancellationToken);

        OllamaChatResponse? ollamaResponse = await DeserializeResponseAsync(response, cancellationToken);

        return MapResponse(ollamaResponse);
    }

    private async Task<OllamaChatResponse> DeserializeResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);

            if (ollamaResponse == null)
            {
                const string message = "Failed to deserialize Ollama API response.";

                _logger.LogError(message);

                throw new AIResponseException($"{nameof(OllamaChatResponse)}" + message, 
                    response.StatusCode, 
                    _aiOptions.Provider);
            }

            return ollamaResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error occurred while deserializing Ollama API response");
            throw new AIResponseException("Error occurred while deserializing Ollama API response", 
                ex, 
                response.StatusCode, 
                _aiOptions.Provider);
        }
        catch(NotSupportedException ex)
        {
            _logger.LogError(ex, "The content type of the Ollama API response is not supported for deserialization");
            throw new AIResponseException("The content type of the Ollama API response is not supported for deserialization", 
                ex, 
                response.StatusCode,
                _aiOptions.Provider);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("Deserialization of Ollama API response was canceled");
            throw new AIRequestTimeoutException("Deserialization of Ollama API response was canceled", 
                ex,
                _aiOptions.Provider);
        }
    }

    private static ChatResponse MapResponse(OllamaChatResponse ollamaResponse)
    {
        return new ChatResponse(
                Content: ollamaResponse.Message.Content,
                Model: ollamaResponse.Model,
                ErrorMessage: null,
                TokensUsed: 0,
                PromptTokensUsed: 0);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(OllamaChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending request to Ollama API with Model: {Model}", _aiOptions.Model);

            var response = await _httpClient.PostAsJsonAsync(chatEndpoint, request, cancellationToken);

            // response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Ollama API request failed with status code {StatusCode}: {ErrorContent}", 
                    response.StatusCode, 
                    errorContent);

                throw new AIResponseException($"Ollama API request failed with status code {response.StatusCode}", 
                    response.StatusCode,
                    _aiOptions.Provider);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error occurred while sending request to Ollama API");
            throw new AIProviderUnavailableException("Unable to reach Ollama API", 
                ex,
                _aiOptions.Provider);
        }
        catch(TaskCanceledException ex)
        {
            _logger.LogWarning("Request to Ollama API was canceled");
            throw new AIProviderUnavailableException("Request to Ollama API was canceled",
                ex,
                _aiOptions.Provider);
        }
    }

    private OllamaChatRequest BuildRequest(IEnumerable<ChatMessage> messages)
    {
        return new OllamaChatRequest
        (
            _aiOptions.Model,
            messages.ToOllamaMessages(),
            false
        );
    }
}
