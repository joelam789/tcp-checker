using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NPoco;

namespace TcpChecker
{
    public static class DbHelper
    {
        static IDictionary<string, string> m_DbSettings = new Dictionary<string, string>();
        public static void LoadDbSettings(IConfiguration configuration)
        {
            m_DbSettings.Clear();
            var cnnstrs = configuration.GetSection("ConnectionStrings").GetChildren();
            foreach (var cnnstr in cnnstrs) m_DbSettings.Add(cnnstr.Key, cnnstr.Value);
        }

        public static IDatabase OpenDb(string dbName)
        {
            return new Database(m_DbSettings[dbName], DatabaseType.MySQL, MySqlClientFactory.Instance);
        }
    }
}
