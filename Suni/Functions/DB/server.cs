using System.Data.SQLite;

namespace Sun.Functions.DB
{
    public partial class DBMethods
    {
        public void InsertServer(ulong serverId, string serverName, string urlIcon, ServerStatusTypes relation = ServerStatusTypes.client,
                                        string flags = "", string eventData = "")
        {
            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertServerQuery = @"
                    INSERT INTO servers (server_id, server_name, url_icon, relation, flags, event_data)
                    VALUES (@serverId, @serverName, @urlIcon, @relation, @flags, @eventData);
                ";

                using (var command = new SQLiteCommand(insertServerQuery, connection))
                {
                    command.Parameters.AddWithValue("@serverId", (long)serverId);
                    command.Parameters.AddWithValue("@serverName", serverName);
                    command.Parameters.AddWithValue("@urlIcon", urlIcon);
                    command.Parameters.AddWithValue("@relation", relation);
                    command.Parameters.AddWithValue("@flags", flags);
                    command.Parameters.AddWithValue("@eventData", eventData);

                    command.ExecuteNonQuery();
                    Console.WriteLine($"add server {serverName} ({serverId})!");
                }
            }
        }
    }
}