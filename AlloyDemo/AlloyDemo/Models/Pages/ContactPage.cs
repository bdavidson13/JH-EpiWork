using System.ComponentModel.DataAnnotations;
using AlloyDemo.Business.Rendering;
using EPiServer.Web;
using EPiServer.Core;
using AlloyDemo.Business.Selectors;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Models.Pages
{
    /// <summary>
    /// Represents contact details for a contact person
    /// </summary>
    [SiteContentType(
        GUID = "F8D47655-7B50-4319-8646-3369BA9AF05B",
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-contact.png")]
    public class ContactPage : SitePageData, IContainerPage
    {
        [Display(GroupName = Global.GroupNames.Contact)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Image { get; set; }

        [Display(GroupName = Global.GroupNames.Contact)]
        public virtual string Phone { get; set; }

        [Display(GroupName = Global.GroupNames.Contact)]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Display(Name = "Region", Order = 10, GroupName = Global.GroupNames.Contact)]
        [SelectOneEnum(typeof(Region))]
        public virtual Region Region { get; set; }

        [Display(Name = "YouTube video", Order = 20, GroupName = Global.GroupNames.Contact)]
        [SelectOne(SelectionFactoryType = typeof(YouTubeSelectionFactory))]
        public virtual string YouTubeVideo { get; set; }

        [Display(Name = "Home city", Order = 30, GroupName = Global.GroupNames.Contact)]
        public virtual string HomeCity { get; set; }

        [Display(Name = "Other cities", Order = 40, GroupName = Global.GroupNames.Contact)]
        public virtual string OtherCities { get; set; }

    }
}
