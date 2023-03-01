using EPiServer.Cms.TinyMce.Core;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace AlloyDemo.Business.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(XhtmlString),
        EditorDescriptorBehavior = EditorDescriptorBehavior.PlaceLast)]
    public class TinyMceEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        { // if not CMS admin then limit capabilities
            if (!PrincipalInfo.CurrentPrincipal.IsInRole("CmsAdmins")) 
            {
                TinyMceSettings settings = metadata.EditorConfiguration["settings"] as TinyMceSettings;

                // default is "tableprops tabledelete | tableinsertrowbefore tableinsertrowafter tabledeleterow | tableinsertcolbefore tableinsertcolafter tabledeletecol"
                settings.AddSetting("table_toolbar", "tabledelete | tableinsertrowbefore tableinsertrowafter tabledeleterow | tableinsertcolbefore tableinsertcolafter tabledeletecol")

                    // the following fields will not appear: Cell spacing, Cell padding, Border and Caption
                    .AddSetting("table_appearance_options", false)

                    // Advanced tab allows a user to input style, border color and background color values.  Hide the Advanced tab like this:
                    .AddSetting("table_advtab", false)
                    .AddSetting("table_row_advtab", false)
                    .AddSetting("table_cell_advtab", false);
            }
        }
    }
}