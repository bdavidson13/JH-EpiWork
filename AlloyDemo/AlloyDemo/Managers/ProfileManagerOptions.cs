using AlloyDemo.Profiles;
using DeaneBarker.Optimizely.ProfileVisitorGroups.TestingCode;
using System;
using System.Collections.Generic;

namespace AlloyDemo.Managers
{
    public class ProfileManagerOptions
    {
        public ProfileManagerOptions()
        {
            ProfileLoaders.Add(SampleLoaders.PopulateProfileFromExternalSource_One);
        }
        public List<Action<IProfile>> ProfileLoaders { get; set; } = new List<Action<IProfile>>();
    }
}
