using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;

namespace Blend.Optimizely.EditorDescriptors
{
    public class EnumSelectionFactory<TEnum> : ISelectionFactory where TEnum : struct, Enum
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var values = Enum.GetValues<TEnum>();
            foreach (var value in values)
            {
                yield return new SelectItem
                {
                    Text = EnumLocalization.GetValueName(value),
                    Value = value
                };
            }
        }
    }
}
