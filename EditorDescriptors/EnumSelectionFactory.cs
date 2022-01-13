using EPiServer.Shell.ObjectEditing;

namespace Blend.Optimizely.EditorDescriptors
{
    public class EnumSelectionFactory<TEnum> : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var values = Enum.GetValues(typeof(TEnum));
            foreach (var value in values)
            {
                yield return new SelectItem
                {
                    Text = EnumLocalization.GetValueName<TEnum>(value),
                    Value = value
                };
            }
        }
    }
}
