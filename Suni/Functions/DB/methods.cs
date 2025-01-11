using System.Data.SQLite;

namespace Sun.Functions.DB
{
    public enum UserStatusTypes{
        client, owner, staff, limited1, banned
    }
    public enum ServerStatusTypes{
        banned, limited1, partnership, client
    }
    public enum NptListeners{
        custom_command,
    }

    public partial class DBMethods
    {
        internal string dbFilePath = "./Suni/Functions/DB/database.db";//used for all
    }
}
