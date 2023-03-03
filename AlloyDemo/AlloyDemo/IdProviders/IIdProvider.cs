using System.Web;

namespace AlloyDemo.IdProviders
{
    public interface IIdProvider
    {
        string GetId();
        void SetContext(HttpContextBase context);
    }
}
