using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System;

namespace AlloyDemo.Features.Favorites
{
    [EPiServerDataStore]
    public class Favorite : IDynamicData
    {
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public string UserName { get; set; }

        [EPiServerDataIndex]
        public ContentReference FavoriteContentReference { get; set; }

        public Favorite()
        {
            Id = Identity.NewIdentity(Guid.NewGuid());
            UserName = string.Empty;
            FavoriteContentReference = ContentReference.EmptyReference;
        }

        public Favorite(ContentReference contentReference,
            string userName) : this() // calls the default constructor first
        {
            UserName = userName;
            FavoriteContentReference = contentReference;
        }
    }
}
