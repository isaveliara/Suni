using System.Data.SQLite;
using Suni.Suni.Functions.DB;
namespace Suni.Suni.Functions
{
    public partial class RomanceMethods
    {
        private const int MarriageInitialCost = 10000;
        private const int DailyMarriageCost = 200;

        public static bool MarryUsers(ulong userId1, ulong userId2, bool splitFinances)
        {
            var db = new DBMethods();
            using (var connection = new SQLiteConnection($"Data Source={db.dbFilePath};Version=3;"))
            {
                connection.Open();

                //check and try remove
                int halfCost = MarriageInitialCost / 2;
                bool deductedUser1 = db.CheckAndDeductBalance((long)userId1, halfCost);
                bool deductedUser2 = db.CheckAndDeductBalance((long)userId2, halfCost);

                if (!deductedUser1 || !deductedUser2)
                {
                    Console.WriteLine("Saldo insuficiente para um ou ambos os usuários.");
                    return false;
                }

                string updateMarriageStatus = @"
                    UPDATE users 
                    SET married_with = @partnerId,
                        flags = @flags
                    WHERE user_id = @userId;";

                string flags = splitFinances ? "spl" : "";//TODO: update this flag sys.

                using (var command = new SQLiteCommand(updateMarriageStatus, connection))
                {
                    command.Parameters.AddWithValue("@partnerId", userId2);
                    command.Parameters.AddWithValue("@userId", userId1);
                    command.Parameters.AddWithValue("@flags", flags);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@partnerId", userId1);
                    command.Parameters.AddWithValue("@userId", userId2);
                    command.Parameters.AddWithValue("@flags", flags);
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Users {userId1} and {userId2} are mared! Cust applide");
                return true;
            }
        }
    }
}
