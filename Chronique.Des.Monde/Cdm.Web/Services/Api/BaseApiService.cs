using System.Net.Http.Json;
using System.Text.Json;

namespace Cdm.Web.Services.Api;

/// <summary>
/// Service de base pour les appels API avec gestion des erreurs et JSON
/// </summary>
public class BaseApiService
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public BaseApiService(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Effectue un GET et désérialise automatiquement la réponse
    /// </summary>
    protected async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            _logger.LogDebug("🌐 GET {Endpoint}", endpoint);
            
            var response = await _httpClient.GetAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                _logger.LogDebug("✅ GET {Endpoint} - Success", endpoint);
                return result;
            }
            
            _logger.LogWarning("⚠️ GET {Endpoint} - {StatusCode}: {Reason}", 
                endpoint, response.StatusCode, response.ReasonPhrase);
                
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ GET {Endpoint} - Exception", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Effectue un POST avec sérialisation automatique
    /// </summary>
    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            _logger.LogDebug("🌐 POST {Endpoint}", endpoint);
            
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, JsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
                _logger.LogDebug("✅ POST {Endpoint} - Success", endpoint);
                return result;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("⚠️ POST {Endpoint} - {StatusCode}: {Error}", 
                endpoint, response.StatusCode, errorContent);
                
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ POST {Endpoint} - Exception", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Effectue un PUT avec sérialisation automatique
    /// </summary>
    protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            _logger.LogDebug("🌐 PUT {Endpoint}", endpoint);
            
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, JsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
                _logger.LogDebug("✅ PUT {Endpoint} - Success", endpoint);
                return result;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("⚠️ PUT {Endpoint} - {StatusCode}: {Error}", 
                endpoint, response.StatusCode, errorContent);
                
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ PUT {Endpoint} - Exception", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Effectue un DELETE
    /// </summary>
    protected async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            _logger.LogDebug("🌐 DELETE {Endpoint}", endpoint);
            
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ DELETE {Endpoint} - Success", endpoint);
                return true;
            }
            
            _logger.LogWarning("⚠️ DELETE {Endpoint} - {StatusCode}: {Reason}", 
                endpoint, response.StatusCode, response.ReasonPhrase);
                
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ DELETE {Endpoint} - Exception", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Effectue un POST sans contenu de réponse attendu
    /// </summary>
    protected async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            _logger.LogDebug("🌐 POST {Endpoint}", endpoint);
            
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, JsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ POST {Endpoint} - Success", endpoint);
                return true;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("⚠️ POST {Endpoint} - {StatusCode}: {Error}", 
                endpoint, response.StatusCode, errorContent);
                
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ POST {Endpoint} - Exception", endpoint);
            throw;
        }
    }
}