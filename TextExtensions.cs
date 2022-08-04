namespace Blend.Optimizely
{
    public static class TextExtensions
    {
        const string LF = "\n";
        const string CR = "\r";
        const string CRLF = "\r\n";
        const string BR = "<br />";

        public static string NormalizeNewlines(this string value)
            => value.Replace(CRLF, LF)
                .Replace(CR, LF);

        public static string NewlinesToBreaks(this string value)
            => value.Replace(LF, BR);

        public static string EncodeHtml(this string value)
            => System.Net.WebUtility.HtmlEncode(value);

        public static string? ConvertTextAreaToHtml(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.EncodeHtml()
                .NormalizeNewlines()
                .NewlinesToBreaks();
        }
    }
}
