using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sun.Globalization;

namespace Sun.Functions.DB
{
    public partial class Methods
    {

        //Checks an user balance, and deducts a value (includes partner)

        public bool CheckAndDeductBalance(long userId, int amountRequired)
        {
            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
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
                    using (var reader = command.ExecuteReader())
                    {
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
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                partnerBalance = Convert.ToInt32(reader["balance"]);
                            else
                            {
                                Console.WriteLine("unknow married user");
                                return false;
                            }
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
        }

        private static void DeductBalance(long userId, int amount, SQLiteConnection connection)
        {
            string deductBalanceQuery = "UPDATE users SET balance = balance - @amount WHERE user_id = @userId;";
            using (var command = new SQLiteCommand(deductBalanceQuery, connection))
            {
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@userId", userId);
                command.ExecuteNonQuery();
            }
        }


        //inserts a user on db
        public void InsertUser(ulong userId, string username, string avatarUrl,
                        ulong? marriedWith = null, ulong balance = 0,
                        string flags = "", string badges = "user", string eventData = "",
                        SuniSupportedLanguages primaryLang = SuniSupportedLanguages.FROM_CLIENT,
                        UserStatusTypes status = UserStatusTypes.client,
                        int xp = 0, int reputation = 0, ulong commandNu = 0, DateTime? lastActive = null, bool DBInsertOrIgnore = true)
        {
            lastActive = lastActive ?? DateTime.Now;
            string value = DBInsertOrIgnore ? "INSERT OR IGNORE" : "INSERT";

            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertUserQuery = value + @" INTO users (user_id, username, avatar_url, married_with, balance, flags, badges,
                                       event_data, primary_lang, status, xp, reputation, commandNu, last_active)
                    VALUES (@userId, @username, @avatarUrl, @marriedWith, @balance, @flags, @badges,
                            @eventData, @primaryLang, @status, @xp, @reputation, @commandNu, @lastActive);
                ";

                using (var command = new SQLiteCommand(insertUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", (long)userId);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@avatarUrl", avatarUrl);
                    command.Parameters.AddWithValue("@marriedWith", marriedWith.HasValue ? (long)marriedWith.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@balance", balance);
                    command.Parameters.AddWithValue("@flags", flags);
                    command.Parameters.AddWithValue("@badges", badges);
                    command.Parameters.AddWithValue("@eventData", eventData);
                    command.Parameters.AddWithValue("@primaryLang", primaryLang.ToString());
                    command.Parameters.AddWithValue("@status", status.ToString());
                    command.Parameters.AddWithValue("@xp", xp);
                    command.Parameters.AddWithValue("@reputation", reputation);
                    command.Parameters.AddWithValue("@commandNu", commandNu);
                    command.Parameters.AddWithValue("@lastActive", lastActive);

                    command.ExecuteNonQuery();
                    Console.WriteLine($"add user {username} ({userId})!");
                }
            }
        }

        //updates the selected user fields

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


        //TODO: work for users and servers
        //returns the selected values

        public Dictionary<string, object> GetUserFields(ulong userId, List<string> fieldsToRetrieve)
        {
            if (fieldsToRetrieve == null || fieldsToRetrieve.Count == 0)
                throw new Exception("No fields to retrieve were provided.");

            string selectQuery = $"SELECT {string.Join(", ", fieldsToRetrieve)} FROM users WHERE user_id = @userId;";

            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", (long)userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            throw new Exception($"No user found with user_id {userId}");

                        reader.Read();
                        var result = new Dictionary<string, object>();

                        foreach (var field in fieldsToRetrieve)
                            result[field] = reader[field] is DBNull ? null : reader[field];

                        return result;
                    }
                }
            }
        }

        //returns true if user exists

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
    }
}