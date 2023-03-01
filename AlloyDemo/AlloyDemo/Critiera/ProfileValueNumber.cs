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

namespace DeaneBarker.Optimizely.ProfileVisitorGroups.Criteria
{

    public class ProfileValueNumberSettings : CriterionModelBase, IValidateCriterionModel
    {
        [Required]
        public string Key { get; set; }

        [DojoWidget(SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueNumber>))]

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
        DisplayName = "Profile Value: Number",
        Description = "Compare a number value from the visitor's profile"
    )]
    public class ProfileValueNumber : CriterionBase<ProfileValueNumberSettings>, IExposesOperators
    {
        public override bool IsMatch(IPrincipal principal, HttpContextBase httpContext)
        {
            var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
            var number = profileManager.GetInt(Model.Key);
            if(number == null) return false; // Wasn't a number...

            return Comparisons[Model.Operator](Model.Value, number.Value);
        }

        public Dictionary<string, Func<int, int, bool>> Comparisons { get; } = new Dictionary<string, Func<int, int, bool>>()
        {
            [">"] = (setting, fromProfile) => { return fromProfile > setting; },
            ["<"] = (setting, fromProfile) => { return fromProfile < setting; },
            [">="] = (setting, fromProfile) => { return fromProfile >= setting; },
            ["<="] = (setting, fromProfile) => { return fromProfile <= setting; },
            ["="] = (setting, fromProfile) => { return fromProfile == setting; },
            ["<>"] = (setting, fromProfile) => { return fromProfile != setting; }
        };

        public List<string> Operators => Comparisons.Keys.ToList();

        List<string> IExposesOperators.Operators { get => this.Operators; set => Comparisons.Keys.ToList(); }
    }
}