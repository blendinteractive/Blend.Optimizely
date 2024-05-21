using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Blend.Optimizely.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "classes")]
    public class ClassListHelper : TagHelper
    {
        public IEnumerable<string>? Classes { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Classes.HasValue() && context != null && output != null)
            {
                var classes = Classes.ToList();
                var existingClasses = context.AllAttributes["class"]?.Value.ToString();
                if (existingClasses != null)
                {
                    var existingClassList = existingClasses.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);
                    classes.AddRange(existingClassList);
                }
                if (output.Attributes.TryGetAttribute("class", out var outputClass))
                {
                    var outputClassList = outputClass?.Value?.ToString()?.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);
                    if (outputClassList != null && outputClassList.HasValue())
                        classes.AddRange(outputClassList);
                }
                output.Attributes.SetAttribute("class", string.Join(" ", classes));
            }
        }
    }
}
