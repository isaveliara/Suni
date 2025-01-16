using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Returns true if one of users are married
    /// </summary>
    public bool AreUsersMarried(ulong userId1, ulong userId2)
    {
        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            connection.Open();
            string query = @"
                SELECT COUNT(1)
                FROM users
                WHERE (user_id = @userId1 OR user_id = @userId2)
                AND married_with IS NOT NULL
                AND married_with != 0;";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId1", (long)userId1);
                command.Parameters.AddWithValue("@userId2", (long)userId2);

                var result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }
    }
}