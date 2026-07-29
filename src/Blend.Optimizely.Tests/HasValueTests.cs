using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using System.Collections.Generic;
using Xunit;

namespace Blend.Optimizely.Tests;

public class HasValueTests
{
    #region Content Reference
    [Fact]
    public void HasValue_ContentReference_ReturnsFalse_ForNull()
    {
        ContentReference? r = null;
        Assert.False(r.HasValue());
    }

    [Fact]
    public void HasValue_ContentReference_ReturnsFalse_ForEmptyReference()
    {
        Assert.False(ContentReference.EmptyReference.HasValue());
    }

    [Fact]
    public void HasValue_ContentReference_ReturnsFalse_ForDefaultConstructor()
    {
        Assert.False(new ContentReference().HasValue());
    }

    [Fact]
    public void HasValue_ContentReference_ReturnsTrue_ForValidId()
    {
        Assert.True(new ContentReference(5).HasValue());
    }
    #endregion

    #region ContentArea
    [Fact]
    public void HasValueUnfiltered_ContentArea_ReturnsFalse_ForNull()
    {
        ContentArea? area = null;
        Assert.False(area.HasValueUnfiltered());
    }

    [Fact]
    public void HasValueUnfiltered_ContentArea_ReturnsFalse_WhenEmpty()
    {
        Assert.False(new ContentArea().HasValueUnfiltered());
    }
    #endregion

    #region XHtmlString
    [Fact]
    public void HasValue_XhtmlString_ReturnsFalse_ForNull()
    {
        XhtmlString? x = null;
        Assert.False(x.HasValue());
    }

    [Fact]
    public void HasValue_XhtmlString_ReturnsFalse_ForEmpty()
    {
        Assert.False(new XhtmlString("").HasValue());
    }

    [Fact]
    public void HasValue_XhtmlString_ReturnsTrue_ForContent()
    {
        Assert.True(new XhtmlString("<p>hello</p>").HasValue());
    }
    #endregion

    #region string
    [Fact]
    public void HasValue_String_ReturnsFalse_ForNull()
    {
        string? s = null;
        Assert.False(s.HasValue());
    }

    [Fact]
    public void HasValue_String_ReturnsFalse_ForEmpty()
    {
        Assert.False("".HasValue());
    }

    [Fact]
    public void HasValue_String_ReturnsTrue_ForWhitespace()
    {
        Assert.True("   ".HasValue());
    }

    [Fact]
    public void HasValue_String_ReturnsTrue_ForContent()
    {
        Assert.True("hello".HasValue());
    }
    #endregion

    #region Url
    [Fact]
    public void HasValue_Url_ReturnsFalse_ForNull()
    {
        Url? u = null;
        Assert.False(u.HasValue());
    }

    [Fact]
    public void HasValue_Url_ReturnsTrue_ForAbsolute()
    {
        Assert.True(new Url("https://example.com").HasValue());
    }

    [Fact]
    public void HasValue_Url_ReturnsTrue_ForRelativePath()
    {
        Assert.True(new Url("/about").HasValue());
    }
    #endregion

    #region LinkItemCollection
    [Fact]
    public void HasValue_LinkItemCollection_ReturnsFalse_ForNull()
    {
        LinkItemCollection? c = null;
        Assert.False(c.HasValue());
    }

    [Fact]
    public void HasValue_LinkItemCollection_ReturnsFalse_WhenEmpty()
    {
        Assert.False(new LinkItemCollection().HasValue());
    }

    [Fact]
    public void HasValue_LinkItemCollection_ReturnsTrue_WhenPopulated()
    {
        var c = new LinkItemCollection
        {
            new LinkItem { Text = "Home", Href = "/" }
        };
        Assert.True(c.HasValue());
    }
    #endregion

    #region IEnumerable<T>
    [Fact]
    public void HasValue_IEnumerableT_ReturnsFalse_ForNull()
    {
        IEnumerable<int>? e = null;
        Assert.False(e.HasValue());
    }

    [Fact]
    public void HasValue_IEnumerableT_ReturnsFalse_WhenEmpty()
    {
        Assert.False(new List<int>().HasValue());
    }

    [Fact]
    public void HasValue_IEnumerableT_ReturnsTrue_WhenPopulated()
    {
        Assert.True(new List<int> { 1 }.HasValue());
    }
    #endregion

    #region LinkItem
    [Fact]
    public void HasValue_LinkItem_ReturnsFalse_ForNull()
    {
        LinkItem? item = null;
        Assert.False(item.HasValue());
    }

    [Fact]
    public void HasValue_LinkItem_ReturnsFalse_WhenTextMissing()
    {
        Assert.False(new LinkItem { Text = "", Href = "/foo" }.HasValue());
    }

    [Fact]
    public void HasValue_LinkItem_ReturnsFalse_WhenHrefMissing()
    {
        Assert.False(new LinkItem { Text = "Foo", Href = "" }.HasValue());
    }

    [Fact]
    public void HasValue_LinkItem_ReturnsTrue_WhenBothPresent()
    {
        Assert.True(new LinkItem { Text = "Home", Href = "/" }.HasValue());
    }
    #endregion

    #region object
    [Fact]
    public void HasValue_Object_DispatchesToContentReference()
    {
        object r = new ContentReference(5);
        Assert.True(r.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToXhtmlString()
    {
        object x = new XhtmlString("<p>hi</p>");
        Assert.True(x.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToString()
    {
        object s = "hello";
        Assert.True(s.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToUrl()
    {
        object u = new Url("https://example.com");
        Assert.True(u.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToLinkItemCollection()
    {
        var c = new LinkItemCollection();
        c.Add(new LinkItem { Text = "Home", Href = "/" });
        object o = c;
        Assert.True(o.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToLinkItem()
    {
        object item = new LinkItem { Text = "Home", Href = "/" };
        Assert.True(item.HasValue());
    }

    [Fact]
    public void HasValue_Object_ReturnsFalse_ForNull()
    {
        object? o = null;
        Assert.False(o.HasValue());
    }

    [Fact]
    public void HasValue_Object_ReturnsTrue_ForArbitraryNonNull()
    {
        object o = 42;
        Assert.True(o.HasValue());
    }

    [Fact]
    public void HasValue_Object_DispatchesToContentArea()
    {
        object area = new ContentArea();
        Assert.False(area.HasValue());
    }

    [Fact]
    public void HasValue_IEnumerable_DispatchesToIEnumerable()
    {
        System.Collections.IEnumerable area = new List<string>();
        Assert.False(area.HasValue());
    }
    #endregion

}
