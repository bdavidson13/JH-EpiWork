using AlloyDemo.Profiles;
using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Caching.Memory;

namespace AlloyDemo.Stores
{
    public class ProfileStore : IProfileStore
    {
        // This is thread-safe; no need to lock
        private static readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public IProfile Get(string id)
        {
            return cache.Get(id) as DictionaryProfile;
        }

        public void Put(IProfile profile)
        {
            cache.Set(profile.Id, profile);
        }

    }
}
