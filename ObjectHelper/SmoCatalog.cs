using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace ObjectHelper
{
    /// <summary>
    /// Catálogo SMO compartido por las UIs de login (DBCompare y DBDocumentation).
    /// </summary>
    public static class SmoCatalog
    {
        public static List<string> ListServers()
        {
            List<string> names = new List<string>();
            DataTable dt = SmoApplication.EnumAvailableSqlServers(false);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    names.Add(dr["Name"].ToString());
                }
            }
            return names;
        }

        public static List<string> ListDatabases(string server)
        {
            return ListDatabases(server, null, null);
        }

        public static List<string> ListDatabases(string server, string login, string password)
        {
            List<string> names = new List<string>();
            ServerConnection conn = new ServerConnection();
            conn.ServerInstance = server;
            if (!string.IsNullOrEmpty(login))
            {
                conn.LoginSecure = false;
                conn.Login = login;
                conn.Password = password ?? string.Empty;
            }

            Server srv = new Server(conn);
            foreach (Database db in srv.Databases)
            {
                names.Add(db.Name);
            }
            return names;
        }
    }
}
