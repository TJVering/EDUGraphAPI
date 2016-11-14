using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Graph;

namespace EDUGraphAPI
{
    public static class GraphExtensions
    {
        public static async Task<IUser[]> ExecuteAllAsync(this IUserCollection collection)
        {
            var pagedCollection = await collection.ExecuteAsync();
            return await ExecuteAllAsync(pagedCollection);
        }

        public static async Task<T[]> ExecuteAllAsync<T>(this IPagedCollection<T> collection)
        {
            var list = new List<T>();

            var c = collection;
            while (true)
            {
                list.AddRange(c.CurrentPage);
                if (c.MorePagesAvailable) c = await c.GetNextPageAsync();
                else break;
            }
            return list.ToArray();
        }

        public static async Task<bool> AnyAsync<T>(this IPagedCollection<T> collection, Func<T, bool> predicate)
        {
            var c = collection;
            while (true)
            {
                if (c.CurrentPage.Any(predicate)) return true;
                if (c.MorePagesAvailable) c = await c.GetNextPageAsync();
                else break;
            }
            return false;
        }

        public static async Task<T> ExecuteFirstOrDefaultAsync<T>(this IReadOnlyQueryableSet<T> set)
        {
            var items = await set.Take(1).ExecuteAsync();
            return items.CurrentPage.FirstOrDefault();
        }

        public static async Task<Conversation[]> GetAllAsync(this IGroupConversationsCollectionRequest request)
        {
            var collectionPage = await request.GetAsync();
            return await GetAllAsync(collectionPage);
        }

        public static async Task<DriveItem[]> GetAllAsync(this IDriveItemChildrenCollectionRequest request)
        {
            var collectionPage = await request.GetAsync();
            return await GetAllAsync(collectionPage);
        }

        private static async Task<TItem[]> GetAllAsync<TItem>(ICollectionPage<TItem> collectionPage)
        {
            var list = new List<TItem>();

            dynamic page = collectionPage;
            do
            {
                list.AddRange(page.CurrentPage);
                if (page.NextPageRequest == null) break;
                page = await page.NextPageRequest.GetAsync();
            }
            while (true);

            return list.ToArray();
        }
    }
}