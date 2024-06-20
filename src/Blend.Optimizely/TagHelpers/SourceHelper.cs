using EPiServer;
using EPiServer.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace Blend.Optimizely.TagHelpers
{

    [HtmlTargetElement("Source", Attributes = "srcset")]
    public class SourceHelper : TagHelper
    {
        public ContentReference? src { get; set; }

        public bool EagerLoading { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!src.HasValue() && output != null)
            {
                output.SuppressOutput();
                return;
            }

            var image = src.Get<IImageFile>(); // NOTE: This is likely to be a breaking change - Switching from `ImageFile` to `IImageFile`
            var imgUrl = src.ResolveUrl();

            if (image == null || !imgUrl.HasValue())
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.SetAttribute("src", imgUrl);
            output.Attributes.SetAttribute("alt", image.GetAltText()); // NOTE: This is likely to be a breaking change - Switching from `ContentReference` to `IImageFile`

            if (image.Width > 0)
                output.Attributes.SetAttribute("width", image.Width);

            if (image.Height > 0)
                output.Attributes.SetAttribute("height", image.Height);

            var imageVariations = image.ImageOptions().Where(x => x != image.Width).Select(x =>
            {
                var url = new UrlBuilder(imgUrl);
                url.QueryCollection.Add("width", x.ToString());
                return (string)url + $" {x}w";
            }).ToList();

            if (imageVariations.HasValue())
            {
                imageVariations.Add($"{imgUrl} {image.Width}w");
                output.Attributes.SetAttribute("srcset", string.Join($", ", imageVariations));
                output.Attributes.SetAttribute("sizes", "100vw");
            }

            if (!EagerLoading)
                output.Attributes.SetAttribute("loading", "lazy");
        }
    }
}