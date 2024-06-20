using Blend.Optimizely;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Blend.Optimizely.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "content-link")]
    public class AnchorTagHelper : TagHelper
    {
        private Injected<LinkResolverService> LinkResolver { get; }

        /// <summary>
        /// Some kind of link that can be resolved via Optimizely. This includes:
        /// IResolvable for special cases
        /// IContent objects
        /// Url objects
        /// LinkItem objects
        /// ContentReference objects
        /// </summary>
        [HtmlAttributeName("content-link")]
        public object ContentLink { get; set; }

        /// <summary>
        /// How should the anchor tag be handle when there is no valid href or the condition is false. 
        /// Options are:
        /// None, ConvertLinkToSpan, SuppressOutput, KeepInnerContent
        /// </summary>
        [HtmlAttributeName("link-options")]
        public LinkOptions LinkOptions { get; set; }

        [HtmlAttributeName("fallback-option")]
        public AnchorFallbackOptions FallbackOption { get; set; }

        /// <summary>
        /// Only output an anchor tag with href when condition is true and a valid link exists. Default is true
        /// </summary>
        [HtmlAttributeName("condition")]
        public bool Condition { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null || output is null)
                return;

            var resolvedLink = LinkResolver.Service.TryResolveLink(ContentLink, LinkOptions);
            if (resolvedLink is not null && resolvedLink.Href.HasValue() && Condition)
            {
                output.Attributes.SetAttribute("href", resolvedLink.Href);

                if (resolvedLink.Target.HasValue() && !context.AllAttributes.TryGetAttribute("target", out _))
                    output.Attributes.SetAttribute("target", resolvedLink.Target);
            }
            else if (FallbackOption is AnchorFallbackOptions.ConvertLinkToSpan)
            {
                output.TagName = "span";
            }
            else if (FallbackOption is AnchorFallbackOptions.SuppressOutput)
            {
                output.SuppressOutput();
                return;
            }
            else if (FallbackOption is AnchorFallbackOptions.KeepInnerContent)
            {
                output.TagName = string.Empty;
                return;
            }
        }
    }

    public enum AnchorFallbackOptions
    {
        None = 0,

        /// <summary>
        /// If url is blank, convert the wrapping a tag to a span
        /// </summary>
        ConvertLinkToSpan = 1,

        /// <summary>
        /// If url is blank, suppress output
        /// </summary>
        SuppressOutput = 2,

        /// <summary>
        /// If url is blank, remove wrapping tag
        /// </summary>
        KeepInnerContent = 3
    }
}