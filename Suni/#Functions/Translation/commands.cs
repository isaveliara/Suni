namespace Sun.Globalization.PT
{
    public partial class SlashCommands
    {
        public static (string, string, string) SimpleCommand_Test()
        {
            return ("ping", "testa a minha latência", "Pong! :ping_pong:\nLatência: &{value}");
        }
    } 
}

namespace Sun.Globalization.EN
{
    public partial class SlashCommands
    {
        public static (string, string, string) SimpleCommand_Test()
        {
            return ("ping", "test my latency", "Pong! :ping_pong:\nLatency: &{value}");
        }
    } 
}
