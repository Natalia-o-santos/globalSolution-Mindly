namespace Mindly.Api.DTOs.Application;

public sealed class LinkDto
{
    public LinkDto(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }

    public string Href { get; }
    public string Rel { get; }
    public string Method { get; }
}
