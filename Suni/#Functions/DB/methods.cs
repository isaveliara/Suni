using System.Data.SQLite;
using System;
using System.Threading.Tasks;

namespace SunFunctions.DB
{
    public enum UserStatusTypes{
        client, owner, staff, limited1, banned
    }
    public enum ServerStatusTypes{
        banned, limited1, partnership, client
    }
    public enum LanguageStatusTypes{
        PT, EN, RU, FROM_CLIENT
    }

    public partial class Methods
    {
        public bool UserExistsInDatabase(ulong userId)
        {
            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;")){
                connection.Open();
                string query = "SELECT COUNT(1) FROM users WHERE user_id = @userId;";
                using (var command = new SQLiteCommand(query, connection)){
                    command.Parameters.AddWithValue("@userId", (long)userId);
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        
        private string dbFilePath = "./Suni/#Functions/DB/database.db";//used for all
    }
}
