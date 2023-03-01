
using EPiServer.Logging;
using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlloyDemo.Business.Selectors
{
    public class CitySelectionFactory : ISelectionFactory
    {
        private static readonly ILogger logger =
            LogManager.GetLogger(typeof(CitySelectionFactory));

        private static List<SelectItem> list = null;

        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
