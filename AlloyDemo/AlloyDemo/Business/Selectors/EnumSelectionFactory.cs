using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Business.Selectors
{
    public class EnumSelectionFactory<TEnum> : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(
            ExtendedMetadata metadata)
        {
            var values = Enum.GetValues(typeof(TEnum));
            foreach (var value in values)
            {
                yield return new SelectItem
                {
                    Text = Enum.GetName(typeof(TEnum), value),
                    Value = value
                };
            }
        }
    }
}
