namespace TestSite.Models.Blocks;


[ContentType(
    DisplayName = "Rich Text",
    GUID = "17d2c3cb-d4a3-44c8-81e8-2f53ef1f9324",
    GroupName = "General Content")]
public class RichTextBlock : BlockData
{
    public virtual XhtmlString? Body { get; set; }
}
