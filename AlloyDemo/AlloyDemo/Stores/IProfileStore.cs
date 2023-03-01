using AlloyDemo.Profiles;

namespace AlloyDemo.Stores
{
    public interface IProfileStore
    {
        IProfile Get(string id);
        void Put(IProfile profile);
    }
}
