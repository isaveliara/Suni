using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Get an NikoSharp by PrimaryKey or Name
    /// </summary>
    public (int? primaryKey, ulong ownerId, string nikosharpName, string nikosharpCode, string listen)? GetScriptByKeyOrName(ulong serverId, int? primaryKey = null, string nikosharpName = null)
    {
        if (primaryKey == null && string.IsNullOrEmpty(nikosharpName))
        {
            Console.WriteLine("Error: primaryKey or nikosharpName must be provided.");
            return null;
        }

        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            connection.Open();

            string query = @"
                SELECT npts.primary_key, npts.owner_id, npts.npt_name, npts.nptcode, npts.listen
                FROM npts
                LEFT JOIN server_npt_access ON npts.primary_key = server_npt_access.npt_key
                LEFT JOIN servers ON server_npt_access.server_id = servers.server_id
                WHERE ";

            List<string> conditions = new List<string>();

            if (primaryKey.HasValue)
                conditions.Add("npts.primary_key = @primaryKey");
            if (!string.IsNullOrEmpty(nikosharpName))
                conditions.Add("npts.npt_name = @nptName");
            
            conditions.Add("servers.server_id = @serverId");

            query += string.Join(" AND ", conditions) + " LIMIT 1;";

            using (var command = new SQLiteCommand(query, connection))
            {
                if (primaryKey.HasValue)
                    command.Parameters.AddWithValue("@primaryKey", primaryKey.Value);
                if (!string.IsNullOrEmpty(nikosharpName))
                    command.Parameters.AddWithValue("@nptName", nikosharpName);
                
                command.Parameters.AddWithValue("@serverId", (long)serverId);

                //execute and read values to return them
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            reader.GetInt32(0), //primary_key
                            (ulong)reader.GetInt64(1), //owner_id
                            reader.GetString(2), //npt_name
                            reader.GetString(3), //nptcode
                            reader.GetString(4)  //listen
                        );
                    }
                }
            }
        }

        return null; //no nptcommand found
    }
}
