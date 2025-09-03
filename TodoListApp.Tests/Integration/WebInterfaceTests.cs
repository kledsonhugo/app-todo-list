using System.Net.Http;

namespace TodoListApp.Tests.Integration;

/// <summary>
/// Testes básicos de interface web
/// Valida se a interface está carregando e acessível
/// </summary>
public class WebInterfaceTests
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5146";

    public WebInterfaceTests()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    /// <summary>
    /// Testa se a página inicial carrega corretamente
    /// </summary>
    public async Task<bool> TestHomepageLoads()
    {
        try
        {
            var response = await _httpClient.GetAsync("/");
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            
            // Verifica se elementos essenciais da interface estão presentes
            return content.Contains("Minha Lista de Tarefas") &&
                   content.Contains("Adicionar Nova Tarefa") &&
                   content.Contains("Filtrar Tarefas") &&
                   content.Contains("script.js") &&
                   content.Contains("styles.css");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa se os endpoints da API estão disponíveis
    /// </summary>
    public async Task<bool> TestApiEndpointAvailability()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/todos");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa se os arquivos estáticos (CSS/JS) estão sendo servidos
    /// </summary>
    public async Task<bool> TestStaticFilesAvailable()
    {
        try
        {
            var cssResponse = await _httpClient.GetAsync("/styles.css");
            var jsResponse = await _httpClient.GetAsync("/script.js");
            
            return cssResponse.IsSuccessStatusCode && jsResponse.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Testa se a documentação Swagger está disponível (se configurada)
    /// </summary>
    public async Task<bool> TestSwaggerDocumentation()
    {
        try
        {
            var response = await _httpClient.GetAsync("/swagger/v1/swagger.json");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}