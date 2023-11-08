using Microsoft.AspNetCore.Http.Extensions;
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder();

builder.Host.UseOrleans(siloBuilder => 
{
    /// SiloBuilder - cria um silo que pode armazenar suas granularidades. Nesse cenário, você usará um cluster localhost
    siloBuilder.UseLocalhostClustering();
});

var app = builder.Build();

/// recuperar uma instância de uma fábrica de granularidades, A Orleans fornece uma fábrica de granularidades padrão que gerencia a 
/// criação e recuperação das granularidades usando seu identificador. para obter a fábrica de granularidades e armazená-la em uma 
/// variável chamada grainFactory
var grainFactory = app.Services.GetRequiredService<IGrainFactory>();

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