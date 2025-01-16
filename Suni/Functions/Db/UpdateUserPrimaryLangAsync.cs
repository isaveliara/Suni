using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    public async Task<bool> UpdateUserPrimaryLangAsync(ulong userId, SuniSupportedLanguages newLang)
    {
        Console.WriteLine($"Updated user language to {newLang}! (for {userId}) - tryFoundUserLangAndSet.cs");
        try
        {
            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE users SET primary_lang = @newLang WHERE user_id = @userId";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@newLang", newLang.ToString());
                    cmd.Parameters.AddWithValue("@userId", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }
}
