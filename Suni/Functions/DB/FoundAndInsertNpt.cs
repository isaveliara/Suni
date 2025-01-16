using System.Data.SQLite;
namespace Sun.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Inserts a not code on db.
    /// </summary>
    public void InsertNptCode(ulong owner_id, string npt_name, string nptcode,
        NptListeners listen, bool DBInsertOrIgnore = true)
    {
        using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
        {
            string value = DBInsertOrIgnore ? "INSERT OR IGNORE" : "INSERT";
            connection.Open();
            string insertUserQuery = value + @" INTO ntps (owner_id, npt_name, nptcode, listen)
                VALUES (@owner_id, @npt_name, @nptcode, @listen);
            ";

            using (var command = new SQLiteCommand(insertUserQuery, connection))
            {
                command.Parameters.AddWithValue("@owner_id", (long)owner_id);
                command.Parameters.AddWithValue("@npt_name", npt_name);
                command.Parameters.AddWithValue("@nptcode", nptcode);
                command.Parameters.AddWithValue("@listen", listen.ToString());

                command.ExecuteNonQuery();
                Console.WriteLine($"add nptcode {npt_name} (by {owner_id}) to DB!");
            }
        }
    }
}
