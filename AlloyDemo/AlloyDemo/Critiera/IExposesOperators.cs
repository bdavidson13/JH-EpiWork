using EPiServer.Personalization.VisitorGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AlloyDemo.Critiera
{
    public interface IExposesOperators
    {
         List<string> Operators { get; set; }
    }

    public class OperatorsSelectionFactory<T> : ISelectionFactory where T : IExposesOperators
    {
        public IEnumerable<SelectListItem> GetSelectListItems(Type propertyType)
        {
            return ((IExposesOperators)Activator.CreateInstance(typeof(T))).Operators.Select(i => new SelectListItem() { Text = i, Value = i });
        }
    }
}
