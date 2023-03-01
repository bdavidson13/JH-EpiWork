using EPiServer.Core;
using EPiServer.Data.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlloyDemo.Features.Favorites
{
    public class FavoriteRepository
    {
        private static DynamicDataStore store
        {
            get
            {
                // creates or gets reference to the store named Favorites
                return DynamicDataStoreFactory.Instance
                    .CreateStore("Favorites", typeof(Favorite));
            }
        }

        public static void Save(Favorite fav)
        {
            if (string.IsNullOrWhiteSpace(fav.UserName))
            {
                throw new NullReferenceException(
                    "Unable to add favorite without user name");
            }
            store.Save(fav);
        }

        public static IEnumerable<Favorite> GetFavoritesForUser(string userName)
        {
            return store.Items<Favorite>()
                .Where(fav => fav.UserName == userName);
        }

        public static IEnumerable<Favorite> GetFavoritesForContent(
            ContentReference contentReference)
        {
            return store.Items<Favorite>()
                .Where(fav => fav.FavoriteContentReference == contentReference);
        }

        public static Favorite GetFavorite(
            ContentReference contentReference, string userName)
        {
            return store.Items<Favorite>().Where(fav => (fav.UserName == userName) &&
                    (fav.FavoriteContentReference == contentReference))
                .FirstOrDefault();
        }

        public static void Delete(Favorite fav)
        {
            store.Delete(fav.Id);
        }
    }
}

