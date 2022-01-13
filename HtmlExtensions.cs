using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace Blend.Optimizely
{
    public static class HtmlExtensions
    {
        #region Property

        public static HtmlString PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> model, Expression<Func<TModel, TResult>> expression, string htmlTagName) =>
            PropertyTemplateFor(model, expression, htmlTagName, null);

        public static HtmlString PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> model, Expression<Func<TModel, TResult>> expression, string htmlTagName, object htmlAttributes) =>
            PropertyTemplateFor(model, expression, htmlTagName, string.Empty, htmlAttributes);


        public static HtmlString PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> model, Expression<Func<TModel, TResult>> expression, string htmlTagName, string innerHtmlTagName, object htmlAttributes)
        {
            var modelExpressionProvider = ServiceLocator.Current.GetInstance<ModelExpressionProvider>();
            var metadata = modelExpressionProvider.CreateModelExpression(model.ViewData, expression);
            var hasValue = metadata.Model != null;

            var contextModeResolver = ServiceLocator.Current.GetInstance<IContextModeResolver>();
            // edit mode (with or without value)
            if (contextModeResolver.CurrentMode == ContextMode.Edit)
            {
                return RenderPropertyForEditMode(htmlTagName, innerHtmlTagName, htmlAttributes, metadata.Metadata);
            }
            // view mode with value
            if (hasValue)
            {
                return RenderPropertyForViewMode(htmlTagName, innerHtmlTagName, htmlAttributes, metadata.Metadata);
            }
            // view mode without value
            return HtmlString.Empty;
        }

        private static HtmlString RenderPropertyForEditMode(string tagName, string innerTagName, object htmlAttributes, ModelMetadata metadata)
        {
            var tagBuilder = new TagBuilder(tagName.Coalesce("span"));

            tagBuilder.MergeAttribute("data-epi-property-name", metadata.PropertyName);
            tagBuilder.MergeAttribute("data-epi-use-mvc", "True");

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(attributes);
            }

            var hasValue = metadata != null;
            if (innerTagName.HasValue())
            {
                if (hasValue)
                {
                    var innerTagBuilder = new TagBuilder(innerTagName);
                    innerTagBuilder.InnerHtml.AppendHtml(metadata.ToString());
                    tagBuilder.InnerHtml.AppendHtml(innerTagBuilder.ToString());
                }
            }
            else
            {
                if (hasValue)
                    tagBuilder.InnerHtml.AppendHtml(metadata.ToString());
            }

            return new HtmlString(tagBuilder.ToString());
        }

        private static HtmlString RenderPropertyForViewMode(string tagName, string innerTagName, object htmlAttributes, ModelMetadata metadata)
        {
            var tagBuilder = new TagBuilder(tagName.Coalesce("span"));

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(attributes);
            }

            if (innerTagName.HasValue())
            {
                var innerTagBuilder = new TagBuilder(innerTagName);
                innerTagBuilder.InnerHtml.AppendHtml(metadata.ToString());
                tagBuilder.InnerHtml.AppendHtml(innerTagBuilder.ToString());
            }
            else
                tagBuilder.InnerHtml.AppendHtml(metadata.ToString());

            return new HtmlString(tagBuilder.ToString());
        }

        #endregion Property

        public static string GetString(this IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}