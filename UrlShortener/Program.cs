using Microsoft.AspNetCore.Http.Extensions;
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder();

builder.Host.UseOrleans(siloBuilder => 
{
    /// SiloBuilder - cria um silo que pode armazenar suas granularidades. Nesse cenário, você usará um cluster localhost
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");
    siloBuilder.UseDashboard(options => 
    {
      options.Username = "ADMIN";
      options.Password = "ADMIN";
      options.Host = "*";
    //   options.Port = 5124;
      options.HostSelf = true;
      options.CounterUpdateIntervalMs = 1000;
    });
    // siloBuilder.AddAzureBlobGrainStorage("urls",
    //     // Recommended: Connect to Blob Storage using DefaultAzureCredential
    //     options =>
    //     {
    //         options.ConfigureBlobServiceClient(new Uri("https://<your-account-name>.blob.core.windows.net"),
    //             new DefaultAzureCredential());
    //     });
        // Connect to Blob Storage using Connection strings
        // options => options.ConfigureBlobServiceClient(connectionString));
});

var app = builder.Build();

/// recuperar uma instância de uma fábrica de granularidades, A Orleans fornece uma fábrica de granularidades padrão que gerencia a 
/// criação e recuperação das granularidades usando seu identificador. para obter a fábrica de granularidades e armazená-la em uma 
/// variável chamada grainFactory
var grainFactory = app.Services.GetRequiredService<IGrainFactory>();


/// Endpoints
app.Map("/dashboard", x => x.UseOrleansDashboard());

app.MapGet("/", () => "Hello World!");

app.MapGet("/shorten/{*path}", async (IGrainFactory grains, HttpRequest request, string path) =>
{
    var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");
    var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
    await shortenerGrain.SetUrl(shortenedRouteSegment, path);
    var resultBuilder = new UriBuilder(request.GetEncodedUrl())
    {
        Path = $"/go/{shortenedRouteSegment}"
    };

    return Results.Ok(resultBuilder.Uri);
});

app.MapGet("/go/{shortenedRouteSegment}",
    async (IGrainFactory grains, string shortenedRouteSegment) =>
{
    var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
    var url = await shortenerGrain.GetUrl();

    return Results.Redirect(url);
});

app.Run();