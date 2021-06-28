using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TcpChecker
{
    public class CacheHelper
    {
        static IDictionary<string, ConnectionMultiplexer> m_CacheCnns = new Dictionary<string, ConnectionMultiplexer>();
        public static void LoadCacheSettings(IConfiguration configuration)
        {
            m_CacheCnns.Clear();
            var cnnstrs = configuration.GetSection("CacheConnectionStrings").GetChildren();
            foreach (var cnnstr in cnnstrs)
            {
                m_CacheCnns.Add(cnnstr.Key, ConnectionMultiplexer.Connect(cnnstr.Value));
            }
        }

        public static IDatabase OpenCache(string dbName)
        {
            return m_CacheCnns[dbName].GetDatabase();
        }
    }
}
