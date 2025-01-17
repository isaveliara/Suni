using System.Collections.Generic;
using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Get an Npt by PrimaryKey or Name
    /// </summary>
    public (int? primaryKey, ulong ownerId, string nptName, string nptCode, string listen)? GetNptByKeyOrName(int? primaryKey = null, string nptName = null, ulong? serverId = null)
    {
        if (primaryKey == null && string.IsNullOrEmpty(nptName) && serverId == null)
            return null;

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
            if (!string.IsNullOrEmpty(nptName))
                conditions.Add("npts.npt_name = @nptName");
            if (serverId.HasValue)
                conditions.Add("servers.server_id = @serverId");

            query += string.Join(" AND ", conditions) + " LIMIT 1;";

            using (var command = new SQLiteCommand(query, connection))
            {
                if (primaryKey.HasValue)
                    command.Parameters.AddWithValue("@primaryKey", primaryKey.Value);
                if (!string.IsNullOrEmpty(nptName))
                    command.Parameters.AddWithValue("@nptName", nptName);
                if (serverId.HasValue)
                    command.Parameters.AddWithValue("@serverId", (long)serverId.Value);

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
