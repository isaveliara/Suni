using System.Data.SQLite;
namespace Sun.Functions.DB;

public partial class DBMethods
{
    public void InsertServer(ulong serverId, string serverName, string urlIcon, ServerStatusTypes relation = ServerStatusTypes.client,
                                    string flags = "", string eventData = "", bool InsertOrIgnore = true)
    {
        string value = InsertOrIgnore ? "INSERT OR IGNORE" : "INSERT";

        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            connection.Open();

            // Comando para inserir o servidor
            string insertServerQuery = value + @" INTO servers (server_id, server_name, url_icon, relation, flags, event_data)
                                                  VALUES (@serverId, @serverName, @urlIcon, @relation, @flags, @eventData);";

            using (var command = new SQLiteCommand(insertServerQuery, connection))
            {
                command.Parameters.AddWithValue("@serverId", (long)serverId);
                command.Parameters.AddWithValue("@serverName", serverName);
                command.Parameters.AddWithValue("@urlIcon", urlIcon);
                command.Parameters.AddWithValue("@relation", relation);
                command.Parameters.AddWithValue("@flags", flags);
                command.Parameters.AddWithValue("@eventData", eventData);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    Console.WriteLine($"Warning: Server {serverName} ({serverId}) was not inserted. It may already exist.");
                }
                else
                {
                    Console.WriteLine($"Server {serverName} ({serverId}) successfully added.");
                }
            }

            // Associa os NPTs padrão ao servidor
            string associateDefaultNpts = @"INSERT OR IGNORE INTO server_npt_access (server_id, npt_key)
                                            SELECT @serverId, primary_key
                                            FROM npts
                                            WHERE primary_key BETWEEN 1 AND 2;";

            using (var command = new SQLiteCommand(associateDefaultNpts, connection))
            {
                command.Parameters.AddWithValue("@serverId", (long)serverId);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Default NPTs assigned to server {serverName} ({serverId}).");
        }
    }
}
