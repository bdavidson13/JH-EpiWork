using AlloyDemo.IdProviders;
using AlloyDemo.Profiles;
using System;
using System.Collections.Generic;

namespace AlloyDemo.Managers
{
    public interface IProfileManager
    {
        IProfile LoadForCurrentUser();
        string GetString(string key);
        int? GetInt(string key);
        DateTime? GetDate(string key);
        //DateOnly? GetDate(string key);
        void Save(IProfile profile);
        void Update(string id, Dictionary<string, string> data);
    }
}