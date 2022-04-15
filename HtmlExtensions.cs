using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace Blend.Optimizely
{
    public static class HtmlExtensions
    {
        #region PropertyTemplateFor

        /// <summary>
        /// Similar to PropertyFor, but allows you to wrap the result in a different tag.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="model"></param>
        /// <param name="expression"></param>
        /// <param name="htmlTagName">Tag to wrap result in</param>
        /// <returns></returns>
        public static IHtmlContent PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> model, Expression<Func<TModel, TResult>> expression, string htmlTagName) =>
            PropertyTemplateFor(model, expression, htmlTagName, null);

        /// <summary>
        /// Similar to PropertyFor, but allows you to wrap the result in a different tag.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="model"></param>
        /// <param name="expression"></param>
        /// <param name="htmlTagName"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> model, Expression<Func<TModel, TResult>> expression, string htmlTagName, object? htmlAttributes) =>
            PropertyTemplateFor(model, expression, htmlTagName, string.Empty, htmlAttributes);

        /// <summary>
        /// Similar to PropertyFor, but allows you to wrap the result in a different tag.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlTagName"></param>
        /// <param name="innerHtmlTagName"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlContent PropertyTemplateFor<TModel, TResult>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TResult>> expression, string htmlTagName, string innerHtmlTagName, object? htmlAttributes)
        {
            ModelExpression modelExpression = html.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>().CreateModelExpression(html.ViewData, expression);

            var hasValue = modelExpression.Model != null;

            var displayFor = html.DisplayFor(expression);

            var contextModeResolver = ServiceLocator.Current.GetInstance<IContextModeResolver>();

            // edit mode (with or without value)
            if (contextModeResolver.CurrentMode == ContextMode.Edit)
            {
                return RenderPropertyForEditMode(htmlTagName, innerHtmlTagName, htmlAttributes, modelExpression.Metadata.PropertyName, displayFor);
            }

            // view mode with value
            if (hasValue)
            {
                return RenderPropertyForViewMode(htmlTagName, innerHtmlTagName, htmlAttributes, displayFor);
            }

            // view mode without value
            return HtmlString.Empty;
        }

        private static IHtmlContent RenderPropertyForEditMode(string tagName, string innerTagName, object? htmlAttributes, string propertyName, IHtmlContent content)
        {
            var tagBuilder = new TagBuilder(tagName.Coalesce("span"));

            tagBuilder.MergeAttribute("data-epi-property-name", propertyName);
            tagBuilder.MergeAttribute("data-epi-use-mvc", "True");

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(attributes);
            }

            var hasValue = content?.ToString() != null;
            if (innerTagName.HasValue() && hasValue)
            {
                var innerTagBuilder = new TagBuilder(innerTagName);
                innerTagBuilder.InnerHtml.AppendHtml(content);
                tagBuilder.InnerHtml.AppendHtml(innerTagBuilder);
            }
            else if (hasValue)
            {
                tagBuilder.InnerHtml.AppendHtml(content);
            }

            return tagBuilder;
        }

        private static IHtmlContent RenderPropertyForViewMode(string tagName, string innerTagName, object? htmlAttributes, IHtmlContent content)
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
                innerTagBuilder.InnerHtml.AppendHtml(content);
                tagBuilder.InnerHtml.AppendHtml(innerTagBuilder);
            }
            else
            {
                tagBuilder.InnerHtml.AppendHtml(content);
            }

            return tagBuilder;
        }

        #endregion Property

        /// <summary>
        /// Converts IHtmlContent into a string.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetString(this IHtmlContent content)
        {
            using var writer = new System.IO.StringWriter();
            content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}