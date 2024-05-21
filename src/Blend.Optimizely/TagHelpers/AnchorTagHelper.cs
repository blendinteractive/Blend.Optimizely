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

        [HtmlAttributeName("link-options")]
        public LinkOptions LinkOptions { get; set; }

        [HtmlAttributeName("output-options")]
        public OutputOptions OutputOptions { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var resolvedLink = LinkResolver.Service.TryResolveLink(ContentLink, LinkOptions);

            if (resolvedLink is null)
                return;

            if (resolvedLink.Href.HasValue())
            {
                output.Attributes.SetAttribute("href", resolvedLink.Href);
            }
            else if (OutputOptions is OutputOptions.ConvertLinkToSpan)
            {
                output.TagName = "span";
            }
            else if (OutputOptions is OutputOptions.SuppressOutput)
            {
                output.SuppressOutput();
                return;
            }

            if (resolvedLink.Target.HasValue() && !context.AllAttributes.TryGetAttribute("target", out _))
                output.Attributes.SetAttribute("target", resolvedLink.Target);
        }
    }


    public enum OutputOptions
    {
        None = 0,

        /// <summary>
        /// If url is blank, convert the wrapping <a> to a <span>
        /// </summary>
        ConvertLinkToSpan = 1,

        /// <summary>
        /// If url is blank, suppress output
        /// </summary>
        SuppressOutput = 2
    }
}
