using AlloyDemo.Critiera;
using AlloyDemo.Managers;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;

namespace AlloyDemo.Criteria
{
    public class ProfileValueTextSettings : CriterionModelBase, IValidateCriterionModel
    {
        [Required]
        
        public string Key { get; set; }
        //[VisitorGroupCriterion(
        //    SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueText>)
        //    )]
        //[CriterionPropertyEditor(
        //    SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueText>),
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/operator"
        //)]
        [DojoWidget(SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueText>))]
        [Required]
        public string Operator { get; set; }

        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/value"
        //)]
        public string Value { get; set; }

        [Display(Name = "Case Sensitive")]
        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/text/casing"
        //)]
        public bool MatchCase { get; set; }

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
        DisplayName = "Profile Value: Text",
        Description = "Compare a text value from the visitor's profile"
    )]
    public class ProfileValueText : CriterionBase<ProfileValueTextSettings>, IExposesOperators
    {
        public override bool IsMatch(IPrincipal principal, HttpContextBase httpContext)
        {
            var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
            var value = profileManager.GetString(Model.Key);
            if(value == null) return false;

            var fromProfile = Model.Value;

            if(!Model.MatchCase)
            {
                value = value.ToLower();
                fromProfile= fromProfile.ToLower();
            }

            return Comparisons[Model.Operator](fromProfile.Trim(), value.Trim());
        }

        public Dictionary<string, Func<string, string, bool>> Comparisons { get; } = new Dictionary<string, Func<string, string, bool>>()
        {
            ["equals"] = (setting, fromProfile) => { return fromProfile == setting; },
            ["contains"] = (setting, fromProfile) => { return fromProfile.Contains(setting); },
            ["starts with"] = (setting, fromProfile) => { return fromProfile.StartsWith(setting); },
            ["ends with"] = (setting, fromProfile) => { return fromProfile.EndsWith(setting); },
            ["is empty"] = (setting, fromProfile) => { return string.IsNullOrWhiteSpace(fromProfile); },
            ["is not empty"] = (setting, fromProfile) => { return !string.IsNullOrWhiteSpace(fromProfile); },
            ["is of pattern"] = (setting, fromProfile) => { return Regex.IsMatch(fromProfile, setting); }
        };

        public List<string> Operators => Comparisons.Keys.ToList();

        List<string> IExposesOperators.Operators { get => this.Operators; set =>  Comparisons.Keys.ToList();  }
    }

}