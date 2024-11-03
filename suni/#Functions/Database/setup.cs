using System.Data.SQLite;
using System;

namespace SunFunctions.DB
{
    public class Setup
    {
        public static void Initialize()
        {
            string dbFilePath = "./suni/#Functions/Database/database.db";
            using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
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
                        balance CHAR(64),
                        flags CHAR(16),
                        badges TEXT,
                        event_data TEXT,
                        primary_lang CHECK(primary_lang IN ('PT', 'EN', 'RU', 'CLIENT')),
                        status TEXT CHECK(status IN ('banned', 'limited1', 'staff', 'owner', 'client')),
                        xp INTEGER DEFAULT 0,
                        reputation INTEGER DEFAULT 0,
                        command_status TEXT);";

                //servers table
                string createServersTable = @"
                    CREATE TABLE IF NOT EXISTS servers (
                        primary_key INTEGER PRIMARY KEY AUTOINCREMENT,
                        server_id INTEGER UNIQUE NOT NULL,
                        server_name TEXT,
                        url_icon TEXT,
                        relation TEXT CHECK(relation IN ('banned', 'partnership')),
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
