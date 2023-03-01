using AlloyDemo.Critiera;
using AlloyDemo.Managers;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace AlloyDemo.Criteria
{
    public class ProfileValueRelativeDateSettings : CriterionModelBase, IValidateCriterionModel
    {
        [Required]
        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/key"
        //)]
        public string Key { get; set; }

        [Required]
        //[CriterionPropertyEditor(
        //    SelectionFactoryType = typeof(DatePartSelectionFactory),
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/relativedate/datepart"
        //)]
        [DojoWidget(SelectionFactoryType = typeof(DatePartSelectionFactory))]

        public DatePart DatePart { get; set; }

        //[CriterionPropertyEditor(
        //    SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueRelativeDate>),
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/operator"
        //)]
        [DojoWidget(SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueRelativeDate>))]
        [Required]
        public string Operator { get; set; }

        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/value"
        //)]
        public int Value { get; set; }

        public override ICriterionModel Copy()
        {
            return ShallowCopy();
        }

        public CriterionValidationResult Validate(VisitorGroup currentGroup)
        {
            return new CriterionValidationResult(true);
        }
    }

    [VisitorGroupCriterion(
        Category = "Profile",
        DisplayName = "Profile Value: Relative Date",
        Description = "Compare a value derived from the elapsed time between now and a date from the visitor's profile"
    )]
    public class ProfileValueRelativeDate : CriterionBase<ProfileValueRelativeDateSettings>, IExposesOperators
    {
        public override bool IsMatch(IPrincipal principal, HttpContextBase httpContext)
        {
            var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
            var fromProfile = profileManager.GetDate(Model.Key);
            if (fromProfile == null) return false;

            var timespan = DateTime.Now - fromProfile.Value;

            //var value = Model.DatePart switch
            //{
            //    DatePart.Years => (int)timespan.TotalDays / 365,
            //    DatePart.Months => (int)timespan.TotalDays / 12,
            //    DatePart.Weeks => (int)timespan.TotalDays / 7,
            //    DatePart.Days => throw new NotImplementedException(),
            //    _ => (int)timespan.TotalDays
            //};

            int value=0;
            switch (Model.DatePart)
            {
                case DatePart.Years:
                    value = (int)timespan.TotalDays / 365;
                    break;
                case DatePart.Months:
                    value = (int)timespan.TotalDays / 12;
                    break;
                case DatePart.Weeks:
                    value = (int)timespan.TotalDays / 7;
                    break;
                case DatePart.Days:
                    throw new NotImplementedException();
            }

            var fromCriteria = Model.Value;

            return Comparisons[Model.Operator](value, fromCriteria);
        }

        public Dictionary<string, Func<int, int, bool>> Comparisons { get; } = new Dictionary<string, Func<int, int, bool>>()
        {
            [">"] = (fromProfile, fromCriteria) => { return fromProfile > fromCriteria; },
            ["<"] = (fromProfile, fromCriteria) => { return fromProfile < fromCriteria; },
            [">="] = (fromProfile, fromCriteria) => { return fromProfile >= fromCriteria; },
            ["<="] = (fromProfile, fromCriteria) => { return fromProfile <= fromCriteria; },
            ["="] = (fromProfile, fromCriteria) => { return fromProfile == fromCriteria; },
            ["<>"] = (fromProfile, fromCriteria) => { return fromProfile != fromCriteria; }
        };

        public List<string> Operators => Comparisons.Keys.ToList();

        List<string> IExposesOperators.Operators { get => this.Operators; set => Comparisons.Keys.ToList(); }
    }

    public enum DatePart
    {
        Years,
        Months,
        Weeks,
        Days
    }

    public class DatePartSelectionFactory : ISelectionFactory
    {
        public IEnumerable<SelectListItem> GetSelectListItems(Type propertyType)
        {
            var options = new List<SelectListItem>();
            foreach(var value in Enum.GetValues(typeof(DatePart)))
            {
                options.Add(new SelectListItem() { Text = value.ToString(), Value = value.ToString() });
            }
            return options;
        }
    }
}