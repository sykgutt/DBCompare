using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DBCompare
{
    public class DatabaseConnect
    {
        string conn1 = string.Empty;
        string conn2 = string.Empty;
        //int option = 0;

        private void conectionDB()
        {
            Utilidades extras = new Utilidades();

            try
            {
                if (extras.GetIni("SetupDB1", "UseIntegrated") == "1")
                {
                    conn1 = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Encrypt=True;TrustServerCertificate=True;", extras.GetIni("SetupDB1", "Server"), extras.GetIni("SetupDB1", "DataBase"));
                }
                else
                {
                    conn1 = String.Format("Data Source={0};Initial Catalog={1};UId={2};Pwd={3};Encrypt=True;TrustServerCertificate=True;", extras.GetIni("SetupDB1", "Server"), extras.GetIni("SetupDB1", "DataBase"), extras.GetIni("SetupDB1", "Usuario"), extras.GetIni("SetupDB1", "Password"));
                }

                if (extras.GetIni("SetupDB2", "UseIntegrated") == "1")
                {
                    conn2 = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Encrypt=True;TrustServerCertificate=True;", extras.GetIni("SetupDB2", "Server"), extras.GetIni("SetupDB2", "DataBase"));
                }
                else
                {
                    conn2 = String.Format("Data Source={0};Initial Catalog={1};UId={2};Pwd={3};Encrypt=True;TrustServerCertificate=True;", extras.GetIni("SetupDB2", "Server"), extras.GetIni("SetupDB2", "DataBase"), extras.GetIni("SetupDB2", "Usuario"), extras.GetIni("SetupDB2", "Password"));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private string OptionServer(int Server)
        {
            conectionDB();
            switch (Server)
            {
                case 1:
                    return conn1;
                case 2:
                    return conn2;
                default:
                    return conn1;
            }
        }

        private string EjecuteScalarString(string SQL, int Option)
        {
            using (SqlConnection connection = new SqlConnection(OptionServer(Option)))
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    connection.Open();
                    return (string)command.ExecuteScalar();
                }
            }
        }

        public List<Data> Datos(int Server, string SQL)
        {
            List<Data> result = new List<Data>();

            SqlDataReader reader = null;
            using (SqlConnection conn = new SqlConnection(OptionServer(Server)))
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = SQL;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Data item = new Data()
                        {
                            Schema = reader["Schema"].ToString(),
                            TableName = reader["TableName"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            Columns = reader["Columns"].ToString(),
                            Value = reader["Data2"].ToString(),
                            ValueByColumns = reader["Data"].ToString()
                        };
                        result.Add(item);
                    }
                }
                conn.Close();
            }
            return result;
        }
    }
}
