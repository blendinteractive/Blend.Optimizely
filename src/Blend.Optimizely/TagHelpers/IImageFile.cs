using EPiServer.Core;
using System.Collections.Generic;

namespace Blend.Optimizely.TagHelpers
{
    internal interface IImageFile :IContent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        
        public string GetAltText();

        public IEnumerable<int> ImageOptions();

    }
}
