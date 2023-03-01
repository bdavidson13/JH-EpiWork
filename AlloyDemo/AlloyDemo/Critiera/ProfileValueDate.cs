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

namespace AlloyDemo.Criteria
{
    public class ProfileValueDateSettings : CriterionModelBase, IValidateCriterionModel
    {
        [Required]
        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/key"
        //)]
        public string Key { get; set; }


        //[CriterionPropertyEditor(
        //    SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueDate>),
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/operator"
        //)]
        [DojoWidget(SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueDate>))]

        [Required]
        public string Operator { get; set; }

        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/value"
        //)]
        public DateTime Value { get; set; }

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
        DisplayName = "Profile Value: Date",
        Description = "Compare a date value from the visitor's profile"
    )]
    public class ProfileValueDate : CriterionBase<ProfileValueDateSettings>, IExposesOperators
    {
        public override bool IsMatch(IPrincipal principal, HttpContextBase httpContext)
        {
            var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
            //var profileManager = new ProfileManager();
            var fromProfile = profileManager.GetDate(Model.Key);
            if (fromProfile == null) return false;

            var fromCriteria =Model.Value;

            return Comparisons[Model.Operator](fromProfile.Value, fromCriteria);
        }


        public Dictionary<string, Func<DateTime, DateTime, bool>> Comparisons { get; } = new Dictionary<string, Func<DateTime, DateTime, bool>>()
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

}