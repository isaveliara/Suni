using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;
public partial class DBMethods
{
    /// <summary>
    /// Inserts a server in my database.
    /// </summary>
    public async Task InsertServerAsync(ulong serverId, string serverName, string urlIcon, ServerStatusTypes relation = ServerStatusTypes.client,
                                    string flags = "", string eventData = "")
    {
        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            await connection.OpenAsync();
            string insertServerQuery = @"
                INSERT INTO servers (server_id, server_name, url_icon, relation, flags, event_data)
                VALUES (@serverId, @serverName, @urlIcon, @relation, @flags, @eventData);
            ";

            using (var command = new SQLiteCommand(insertServerQuery, connection))
            {
                command.Parameters.AddWithValue("@serverId", (long)serverId);
                command.Parameters.AddWithValue("@serverName", serverName);
                command.Parameters.AddWithValue("@urlIcon", urlIcon);
                command.Parameters.AddWithValue("@relation", relation.ToString());
                command.Parameters.AddWithValue("@flags", flags);
                command.Parameters.AddWithValue("@eventData", eventData);

                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"add server {serverName} ({serverId})!");
            }
        }
    }
}
