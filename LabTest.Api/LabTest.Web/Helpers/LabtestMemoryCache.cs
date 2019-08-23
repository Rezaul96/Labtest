using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Web.Helpers
{
    public class LabtestMemoryCache
    {
        //private static readonly ObjectCache Cache = MemoryCache.Default;
        private static readonly object Lock = new object();

        public static void Set<T>(string key, T data, DateTimeOffset? time = null)
        {
            //if (!time.HasValue)
            //    time = DateTimeOffset.Now.AddMinutes(30);

            //Remove(key);

            //lock (Lock)
            //    Cache.Set(key, data, time.Value);
            throw new NotImplementedException();
        }

        public static T Get<T>(string key, bool updateExpairation = false, DateTimeOffset? time = null)
        {
            //object item;
            //lock (Lock)
            //{
            //    item = Cache[key];
            //    if (item == null)
            //        return default(T);
            //}
            //if (updateExpairation)
            //{
            //    Set(key, item, time);
            //}

            //return (T)item;
            throw new NotImplementedException();
        }

        public static void Remove(string key)
        {
            //lock (Lock)
            //    if (Cache.Contains(key))
            //        Cache.Remove(key);
            throw new NotImplementedException();
        }

    }
}
