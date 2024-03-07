namespace Blend.Optimizely
{
    public interface IResolvable
    {
        ResolvedLink Resolve(LinkOptions options);
    }

    public record ResolvedLink(string? Href, string? Target)
    {
        public ResolvedLink AddAdditional(string? additional)
        {
            if (string.IsNullOrWhiteSpace(additional))
                return this;

            return new ResolvedLink((this.Href ?? "") + additional, Target);
        }
    }
}
