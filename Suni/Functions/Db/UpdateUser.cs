using System.Collections.Generic;
using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Updates the selected user fields whith the  'Dictionary<string, object>' format values.
    /// </summary>
    public void UpdateUser(ulong userId, Dictionary<string, object> updatedFields)
    {
        if (updatedFields == null || updatedFields.Count == 0)
            throw new Exception("No fields to update were provided.");

        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            connection.Open();
            var setClauses = new List<string>();

            foreach (var field in updatedFields.Keys)
                setClauses.Add($"{field} = @{field}");

            string updateQuery = $"UPDATE users SET {string.Join(", ", setClauses)} WHERE user_id = @userId;";

            using (var command = new SQLiteCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@userId", (long)userId);

                foreach (var field in updatedFields)
                    command.Parameters.AddWithValue($"@{field.Key}", field.Value ?? DBNull.Value);

                command.ExecuteNonQuery();
                Console.WriteLine($"Updated user {userId} with fields: {string.Join(", ", updatedFields.Keys)}");///////debug
            }
        }
    }
}
