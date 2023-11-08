/// <summary>
/// GrainInterface - Criar a granularidade
/// </summary>
public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task SetUrl(string shortenedRouteSegment, string fullUrl);
    Task<string> GetUrl();
}