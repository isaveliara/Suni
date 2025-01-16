using System.Data.SQLite;
namespace Suni.Suni.Functions.DB;

public partial class DBMethods
{
    /// <summary>
    /// Checks an user balance, and deducts a value (includes partner's balance)
    /// </summary>
    public bool CheckAndDeductBalance(long userId, int amountRequired)
    {
        using var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;");
        connection.Open();

        string checkBalanceAndFlags = @"
                SELECT user_id, balance, married_with, flags 
                FROM users 
                WHERE user_id = @userId;";

        int userBalance = 0;
        long partnerId = -1;
        bool splitFinances = false;

        using (var command = new SQLiteCommand(checkBalanceAndFlags, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                userBalance = Convert.ToInt32(reader["balance"]);
                partnerId = reader["married_with"] != DBNull.Value ? Convert.ToInt64(reader["married_with"]) : -1;
                splitFinances = reader["flags"].ToString().Contains("spl");
            }
            else
            {
                Console.WriteLine("unknow user");
                return false;
            }
        }

        //if the balance is sufficient and there is no division of finances
        if (!splitFinances || partnerId == -1)
        {
            if (userBalance >= amountRequired)
            {
                DeductBalance(userId, amountRequired, connection);
                Console.WriteLine($"User {userId} paid {amountRequired} coins.");
                return true;
            }
            else
            {
                Console.WriteLine("insufficient coins");
                return false;
            }
        }
        else //if finances are divided
        {
            //partner
            int partnerBalance = 0;
            string checkPartnerBalance = "SELECT balance FROM users WHERE user_id = @partnerId;";
            using (var command = new SQLiteCommand(checkPartnerBalance, connection))
            {
                command.Parameters.AddWithValue("@partnerId", partnerId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                    partnerBalance = Convert.ToInt32(reader["balance"]);
                else
                {
                    Console.WriteLine("unknow married user");
                    return false;
                }
            }

            int halfAmount = amountRequired / 2;
            if (userBalance >= halfAmount && partnerBalance >= halfAmount)
            {
                DeductBalance(userId, halfAmount, connection);
                DeductBalance(partnerId, halfAmount, connection);
                Console.WriteLine($"User {userId} and partner {partnerId} paid {amountRequired} coins.");
                return true;
            }
            else
            {
                Console.WriteLine("insufficient coins (divided)");
                return false;
            }
        }
    }

    private static void DeductBalance(long userId, int amount, SQLiteConnection connection)
    {
        string deductBalanceQuery = "UPDATE users SET balance = balance - @amount WHERE user_id = @userId;";
        using var command = new SQLiteCommand(deductBalanceQuery, connection);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@userId", userId);
        command.ExecuteNonQuery();
    }
}