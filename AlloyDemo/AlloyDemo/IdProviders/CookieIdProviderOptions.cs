using Microsoft.AspNetCore.Http;

namespace AlloyDemo.IdProviders
{
    public class CookieIdProviderOptions
    {
        public string CookieName { get; set; } = "contentcloudprofileid";
        public CookieOptions CookieOptions { get; set; } = new CookieOptions();
    }
}
