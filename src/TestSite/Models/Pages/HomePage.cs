using EPiServer.SpecializedProperties;

namespace TestSite.Models.Pages;


[ContentType(
    DisplayName = "Home Page",
    GUID = "aebc8e81-4b65-4d76-9a0d-2a91ab41b6a5",
    GroupName = "General Content")]
public class HomePage : PageData
{
    public virtual ContentArea? Body { get; set; }

    public virtual LinkItemCollection? Links { get; set; }

    public virtual Url? TestUrl { get; set; }
}
