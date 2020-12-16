using System.Data.SqlClient;

namespace ReprocessaArqStone
{
    public static class ConnectionApp
    {
        public static SqlConnection GetConnectionDB()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @"192.168.100.107\CLIENTE",
                UserID = "sa",
                Password = "sys@36911",
                InitialCatalog = "THEOSDBCENTRAL",
                ConnectTimeout = 60
                
            };

            return new SqlConnection(builder.ConnectionString);
        }
    }
}
