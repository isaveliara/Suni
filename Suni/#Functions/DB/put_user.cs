using System;
using System.Data.SQLite;

namespace SunFunctions.DB
{
    public partial class Methods
    {
        public void InsertUser(ulong userId, string username, string avatarUrl,
                        ulong? marriedWith = null, ulong balance = 0,
                        string flags = "", string badges = "user", string eventData = "",
                        LanguageStatusTypes primaryLang = LanguageStatusTypes.FROM_CLIENT,
                        UserStatusTypes status = UserStatusTypes.client,
                        int xp = 0, int reputation = 0, ulong commandNu = 0, DateTime? lastActive = null)
        {
            lastActive = lastActive ?? DateTime.Now;

            using (var connection = new SQLiteConnection($"Data Source={this.dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertUserQuery = @"
                    INSERT INTO users (user_id, username, avatar_url, married_with, balance, flags, badges,
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
    }
}