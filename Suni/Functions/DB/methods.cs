using System.Data.SQLite;
using System;
using System.Threading.Tasks;

namespace Sun.Functions.DB
{
    public enum UserStatusTypes{
        client, owner, staff, limited1, banned
    }
    public enum ServerStatusTypes{
        banned, limited1, partnership, client
    }

    public partial class DBMethods
    {
        internal string dbFilePath = "./Suni/Functions/DB/database.db";//used for all
    }
}
