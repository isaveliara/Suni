using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    public async Task<SuniSupportedLanguages> GetUserPrimaryLangAsync(ulong userId)
    {
        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;")){
            connection.Open();
            string query = "SELECT primary_lang FROM users WHERE user_id = @userId";
            using (var cmd = new SQLiteCommand(query, connection)){
                cmd.Parameters.AddWithValue("@userId", userId);
                var result = await cmd.ExecuteScalarAsync();
                
                var l = result != null
                    ? GlobalizationMethods.ParseToLanguageSupported(result.ToString())
                    : SuniSupportedLanguages.FROM_CLIENT;
                
                Console.WriteLine($"(maybe error?) Got {l} as user language (for {userId}) - tryFoundUserLangAndSet.cs");
                return l;
            }
        }
    }
}