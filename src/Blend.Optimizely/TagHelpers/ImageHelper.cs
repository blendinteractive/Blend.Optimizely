using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Blend.Optimizely.TagHelpers
{
    [HtmlTargetElement("Image", Attributes = "src", TagStructure = TagStructure.WithoutEndTag)]
    public class ImageHelper : TagHelper
    {
        public ContentReference? src { get; set; }

        [HtmlAttributeName("eager-loading")]
        public bool EagerLoading { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!src.HasValue() && output != null)
            {
                output.SuppressOutput();
                return;
            }

            var imgUrl = src.ResolveUrl();
            if (!imgUrl.HasValue())
            {
                output.SuppressOutput();
                return;
            }

            var imageVariations = new List<string>();

            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            if (loader.TryGet<IImageFile>(src, out var image))
            {
                output.Attributes.SetAttribute("alt", image.GetAltText());

                if (image.Width > 0)
                    output.Attributes.SetAttribute("width", image.Width);

                if (image.Height > 0)
                    output.Attributes.SetAttribute("height", image.Height);

                imageVariations = image.ImageOptions().Where(x => x != image.Width).Select(x =>
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
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.SetAttribute("src", imgUrl);

            if (!EagerLoading)
                output.Attributes.SetAttribute("loading", "lazy");
        }
    }
}