using System.Net.Http;
using System.Text.Json;
using DocProcessing.InspeccionDocumentos.Entities;
using DocProcessing.InspeccionDocumentos.Settings;
using DocProcessing.InspeccionDocumentos.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InspeccionDocumentos;

public class Worker(ILogger<Worker> logger, IOptions<ScheduleSettings> scheduleSettings, IHttpClientFactory httpClientFactory) : BackgroundService
{
    private readonly ILogger<Worker> logger = logger;
    private readonly ScheduleSettings scheduleSettings = scheduleSettings.Value;
    private readonly HttpClient client = httpClientFactory.CreateClient("ServicioRemoto");

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var response = await client.GetAsync("DocKeyword", stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            List<DocKeyword>? docKeys = null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                logger.LogInformation($"CONTENIDO {content}");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                docKeys = JsonSerializer.Deserialize<List<DocKeyword>>(content, options);
            }
            else
            {
                logger.LogWarning("Error en la solicitud: {status}", response.StatusCode);

            }

            if (docKeys is not null)
            {
                await PdfReader.OCRAsync(client, docKeys);
            }
            await Task.Delay(TimeSpan.FromMinutes(scheduleSettings.IntervalInMinutes), stoppingToken);
        }
    }
}
