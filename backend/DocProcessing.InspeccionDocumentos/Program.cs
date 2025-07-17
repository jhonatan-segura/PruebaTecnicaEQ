using DocProcessing.InspeccionDocumentos.Settings;
using DocProcessing.InspeccionDocumentos.Utils;
using InspeccionDocumentos;

var builder = Host.CreateApplicationBuilder(args);

// Configura un objeto utilizando los valores de la sección Schedule del archivo appsettings.json
builder.Services.Configure<ScheduleSettings>(builder.Configuration.GetSection("Schedule"));
// Crea un cliente HTTP con una URI base.
builder.Services.AddHttpClient("ServicioRemoto", client =>
{
    client.BaseAddress = new Uri("https://localhost:7232/");
});

builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<PdfReader>();

var host = builder.Build();
host.Run();