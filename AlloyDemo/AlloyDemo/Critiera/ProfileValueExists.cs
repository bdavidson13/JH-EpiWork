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
    public class ProfileValueExistsSettings : CriterionModelBase, IValidateCriterionModel
    {
        [Required]
        //[CriterionPropertyEditor(
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/key"
        //)]
        public string Key { get; set; }

        //[CriterionPropertyEditor(
        //    SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueExists>),
        //    LabelTranslationKey = "/visitorgroups/profile/criteria/operator"
        //)]
        [DojoWidget(SelectionFactoryType = typeof(OperatorsSelectionFactory<ProfileValueExists>))]

        [Required]
        public string Operator { get; set; }

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
        DisplayName = "Profile Value: Exists",
        Description = "Check that a value from the profile exists or not"
    )]
    public class ProfileValueExists : CriterionBase<ProfileValueExistsSettings>, IExposesOperators
    {
        public override bool IsMatch(IPrincipal principal, HttpContextBase httpContext)
        {
            var profileManager = ServiceLocator.Current.GetInstance<IProfileManager>();
            var value = profileManager.GetString(Model.Key);

            return Comparisons[Model.Operator](value);
        }

        public Dictionary<string, Func<string, bool>> Comparisons { get; } = new Dictionary<string, Func<string, bool>>()
        {
            ["exists"] = (fromProfile) => { return fromProfile != null; },
            ["does not exist"] = (fromProfile) => { return fromProfile == null; },
        };

        public List<string> Operators => Comparisons.Keys.ToList();

        List<string> IExposesOperators.Operators { get => this.Operators; set => Comparisons.Keys.ToList(); }
    }

}