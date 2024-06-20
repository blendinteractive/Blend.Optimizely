using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.Optimizely.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "wrap-if")]
    [HtmlTargetElement("*", Attributes = "suppress-if")]
    public class ConditionalWrapperHelper : TagHelper
    {
        [HtmlAttributeName("wrap-if")]
        public bool WrapIfCondition { get; set; }

        [HtmlAttributeName("suppress-if")]
        public bool SuppressIfCondition { get; set; }


        [HtmlAttributeName("link-options")]
        public LinkOptions LinkOptions { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (SuppressIfCondition)
            {
                output.SuppressOutput();
            }
            if (!WrapIfCondition)
            {
                output.TagName = null;
            }
        }
    }
}
