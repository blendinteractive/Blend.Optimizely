namespace Blend.Optimizely
{
    public interface IResolvable
    {
        ResolvedLink Resolve(LinkOptions options);
    }

    public record ResolvedLink(string? Href, string? Target);
}
