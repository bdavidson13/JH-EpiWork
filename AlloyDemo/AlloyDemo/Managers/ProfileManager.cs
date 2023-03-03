using AlloyDemo.Features.Loaders;
using AlloyDemo.IdProviders;
using AlloyDemo.Profiles;
using AlloyDemo.Stores;
using DeaneBarker.Optimizely.ProfileVisitorGroups.TestingCode;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlloyDemo.Managers
{
    public class ProfileManager : IProfileManager
    {
        public static List<Action<IProfile>> ProfileLoaders { get; set; } = new List<Action<IProfile>>();
        private readonly IProfileStore _store;
        private readonly IIdProvider _idProvider;
        private HttpContextBase context;

        //public ProfileManager(IProfileStore store, IIdProvider idProvider, IOptions<ProfileManagerOptions> options)
        //{
        //    _store = store;
        //    _idProvider = idProvider;
        //    ProfileLoaders.AddRange(options.Value.ProfileLoaders);
        //}
        public ProfileManager(IProfileStore store, IIdProvider idProvider,HttpContextBase context = null)
        {
            _store = store;
            _idProvider = idProvider;

            if (context != null && idProvider.GetType() == typeof(CookieIdProvider))
            {
                _idProvider.SetContext(context);
            }
            ApiLoader apiLoader = new ApiLoader();
            ProfileLoaders.Add(apiLoader.LoadJson);
        }
        public IProfile Load(string id)
        {
            return _store.Get(id);
        }
        public void setHttpContext(HttpContextBase currentContext)
        {
            context = currentContext;
        }

        public virtual IProfile LoadForCurrentUser()
        {

            var id = _idProvider.GetId();
            if (id == null) return null;

            var profile = Load(id);

            if (profile == null)
            {
                // Create a new profile
                // This constructor will populate from the CDP
                profile = ServiceLocator.Current.GetInstance<IProfile>();
                profile.Id = id;

                foreach (var loader in ProfileLoaders)
                {
                    loader.Invoke(profile);
                }

                Save(profile);

                #if DEBUG
                Manifest.Add(id);
                #endif
            }

            return profile;
        }

        public void Update(string id, Dictionary<string, string> data)
        {
            var profile = Load(id);
            if (profile == null) return;

            foreach (var datum in data)
            {
                profile[datum.Key] = datum.Value;
            }

            Save(profile);
        }


        public string GetString(string key)
        {
            var profile = LoadForCurrentUser();
            return profile?.Get(key);
        }


        public int? GetInt(string key)
        {
            var value = GetString(key);
            if (value == null)
            {
                return null;
            }

            if (!int.TryParse(value, out int typedValue))
            {
                return null;
            }

            return typedValue;
        }

        public DateTime? GetDate(string key)
        {
            var value = GetString(key);
            if (value == null)
            {
                return null;
            }

            if (!DateTime.TryParse(value, out DateTime typedValue))
            {
                return null;
            }

            return typedValue;
        }


        public void Save(IProfile profile)
        {
            _store.Put(profile);
        }


        // Returns one of...
        // 1. The value of the identifier cookie that was passed IN
        //// 2. The value of a new identifier cookie that will be passed BACK
        //private string GetIdFromCookie()
        //{
        //    var httpContextAccessor = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();

        //    string id;

        //    var cookie = httpContextAccessor.HttpContext.Request.Cookies[cookieName];
        //    if (cookie != null)
        //    {
        //        // Get the ID that was passed IN
        //        id = cookie.ToString();
        //    }
        //    else
        //    {
        //        // There's no cookie, so...

        //        // Check to see if it's stored as an HttpContext item
        //        // The problem is that during the first request, this id is not in a cookie, so it's not globally available
        //        // So for the first request, we have to force it to be globally available. After this, we send it back as a cookie, and we're good
        //        if (httpContextAccessor.HttpContext.Items[httpContextKey] != null)
        //        {
        //            id = httpContextAccessor.HttpContext.Items[httpContextKey].ToString();
        //        }
        //        else
        //        {
        //            // Create a new ID and passed it BACK
        //            id = Guid.NewGuid().ToString();
        //            httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, id, cookieOptions);
        //            // Note: I haven't manually done anything with cookies in YEARS
        //            // Is this persistent? I think so? If not, you'll need to add some CookieOptions settings to make it persistent

        //            // Put it in the context so it's globally available for the entirety of the request
        //            httpContextAccessor.HttpContext.Items[httpContextKey] = id;
        //        }

        //    }

        //    return id;
        //}

#if DEBUG
                
        // Won't need for prodction
        // This is just so we can list the profiles for /profile/all
        private List<string> Manifest = new List<string>(); // This exists just to keep track of the keys in the cache (we can't iterate cache keys)
        public List<DictionaryProfile> GetAll()
        {
            return Manifest.Select(k => _store.Get(k) as DictionaryProfile).ToList();
        }

#endif
    }
}
