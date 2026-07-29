using EPiServer.Core;
using Xunit;

namespace Blend.Optimizely.Tests;

public class CoalesceTests
{
    #region ContentReference
    [Fact]
    public void Coalesce_ContentReference_ReturnsValue_WhenHasValue()
    {
        var value = new ContentReference(5);
        var fallback = new ContentReference(99);
        Assert.Equal(value, value.Coalesce(fallback));
    }

    [Fact]
    public void Coalesce_ContentReference_ReturnsFallback_WhenNull()
    {
        ContentReference? value = null;
        var fallback = new ContentReference(99);
        Assert.Equal(fallback, value.Coalesce(fallback));
    }

    [Fact]
    public void Coalesce_ContentReference_ReturnsFallback_WhenEmpty()
    {
        var fallback = new ContentReference(99);
        Assert.Equal(fallback, ContentReference.EmptyReference.Coalesce(fallback));
    }
    #endregion

    #region XHtmlString
    [Fact]
    public void Coalesce_XhtmlString_ReturnsValue_WhenHasValue()
    {
        var value = new XhtmlString("<p>original</p>");
        var fallback = new XhtmlString("<p>fallback</p>");
        Assert.Equal(value, value.Coalesce(fallback));
    }

    [Fact]
    public void Coalesce_XhtmlString_ReturnsFallback_WhenNull()
    {
        XhtmlString? value = null;
        var fallback = new XhtmlString("<p>fallback</p>");
        Assert.Equal(fallback, value.Coalesce(fallback));
    }

    [Fact]
    public void Coalesce_XhtmlString_FromString_ReturnsFallback_WhenNull()
    {
        XhtmlString? value = null;
        var result = value.Coalesce("<p>fallback</p>");
        Assert.Equal("<p>fallback</p>", result.ToHtmlString());
    }

    [Fact]
    public void Coalesce_XhtmlString_FromString_ReturnsValue_WhenHasValue()
    {
        var value = new XhtmlString("<p>original</p>");
        var result = value.Coalesce("<p>fallback</p>");
        Assert.Equal(value, result);
    }
    #endregion

    #region Nullable<T>
    [Fact]
    public void Coalesce_Struct_ReturnsValue_WhenSet()
    {
        int? value = 7;
        Assert.Equal(7, value.Coalesce(99));
    }

    [Fact]
    public void Coalesce_Struct_ReturnsFallback_WhenNull()
    {
        int? value = null;
        Assert.Equal(99, value.Coalesce(99));
    }
    #endregion

    #region string
    [Fact]
    public void Coalesce_String_ReturnsValue_WhenHasValue()
    {
        Assert.Equal("hello", "hello".Coalesce("fallback"));
    }

    [Fact]
    public void Coalesce_String_ReturnsFallback_WhenNull()
    {
        string? value = null;
        Assert.Equal("fallback", value.Coalesce("fallback"));
    }

    [Fact]
    public void Coalesce_String_ReturnsFallback_WhenEmpty()
    {
        Assert.Equal("fallback", "".Coalesce("fallback"));
    }
    #endregion
}
