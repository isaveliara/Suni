using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
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

            //codes table
            string createNptsTable = @"
                CREATE TABLE IF NOT EXISTS npts (
                    primary_key INTEGER PRIMARY KEY AUTOINCREMENT,
                    owner_id INTEGER NOT NULL,
                    npt_name TEXT NOT NULL,
                    nptcode TEXT NOT NULL,
                    listen TEXT CHECK(listen IN ('custom_command')) NOT NULL
                );";

            //npt access table
            string serversAndNptTable = @"
                CREATE TABLE IF NOT EXISTS server_npt_access (
                    server_id INTEGER,
                    npt_key INTEGER,
                    FOREIGN KEY (server_id) REFERENCES servers (server_id) ON DELETE CASCADE,
                    FOREIGN KEY (npt_key) REFERENCES npts (primary_key) ON DELETE CASCADE,
                    PRIMARY KEY (server_id, npt_key));";
            
            string insertDefaultNpts = @"
            INSERT OR IGNORE INTO npts (primary_key, npt_name, nptcode, listen)
            VALUES 
                (1, 'hello', 'std::nout() -> Hello World', 'custom_command'),
                (2, 'ping', 'npt::respond(pong :ping_pong:) -> embedded', 'custom_command');";

        using (var command = new SQLiteCommand(insertDefaultNpts, connection))
            command.ExecuteNonQuery();

            using (var command = new SQLiteCommand(createUsersTable, connection))
                command.ExecuteNonQuery();
            using (var command = new SQLiteCommand(createServersTable, connection))
                command.ExecuteNonQuery();
            using (var command = new SQLiteCommand(createNptsTable, connection))
                command.ExecuteNonQuery();
            using (var command = new SQLiteCommand(serversAndNptTable, connection))
                command.ExecuteNonQuery();

            Console.WriteLine("created!");
        }
    }
}
