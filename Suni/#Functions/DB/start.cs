using System.Data.SQLite;
using System;
using System.Threading.Tasks;

namespace Sun.Functions.DB
{
    public partial class Methods
    {
        public void Setup()
        {
            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                //users table
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS users (
                        primary_key INTEGER PRIMARY KEY AUTOINCREMENT,
                        user_id INTEGER UNIQUE NOT NULL,
                        username TEXT,
                        avatar_url TEXT,
                        married_with INTEGER,
                        balance INTEGER DEFAULT 0,
                        flags CHAR(16),
                        badges TEXT,
                        event_data TEXT,
                        primary_lang CHECK(primary_lang IN ('PT', 'EN', 'RU', 'FROM_CLIENT')),
                        status TEXT CHECK(status IN ('banned', 'limited1', 'staff', 'owner', 'client')),
                        xp INTEGER DEFAULT 0,
                        reputation INTEGER DEFAULT 0,
                        commandNu INTEGER DEFAULT 0,
                        last_active DATETIME);";

                //servers table
                string createServersTable = @"
                    CREATE TABLE IF NOT EXISTS servers (
                        primary_key INTEGER PRIMARY KEY AUTOINCREMENT,
                        server_id INTEGER UNIQUE NOT NULL,
                        server_name TEXT,
                        url_icon TEXT,
                        relation TEXT CHECK(relation IN ('banned', 'limited1', 'partnership','client')),
                        flags CHAR(16),
                        event_data TEXT);";

                using (var command = new SQLiteCommand(createUsersTable, connection))
                    command.ExecuteNonQuery();
                using (var command = new SQLiteCommand(createServersTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("created!");
            }
        }
    }
}
