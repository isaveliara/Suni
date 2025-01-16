using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sun.Functions
{
    public partial class Functions
    {
        public static IEnumerable<int> Dice(int sides = 6, int number = 1)
        {
            if (sides < 1 || number < 1) throw new Exception("Invalid usage of dice. 'sides' and 'number' must be greater than 0.");
            Random random = new Random();

            for (int p = 0; p < number; p++)
                yield return random.Next(1, sides + 1);
        }

        public static string HelloWorld()
            =>
                "Hello World!";

        internal static string GetShipMessage(int percent, string u1, string u2)
            => percent switch
            {
                0 => "...",
                <= 13 => "Esqueça :headskull:",
                <= 24 => "Não existe motivo para que esse casal exista!",
                <= 49 => "Improvável! Vamos torcer por esses dois...",
                <= 69 => "Por que não dar certo? :heart:",
                <= 89 => new Random().Next(1, 3) == 1
                         ? $"O coração de {u1} aquece por {u2}"
                         : $"O coração de {u2} aquece por {u1}",
                90 => "Ownn... Esse seria o casal mais fofinho que eu já vi",
                _ => "?"
            };


        public static string GetSuniStatistics()
        {
            var statistics = new StringBuilder();
            var client = SunClassBot.SuniClient;
            var process = Process.GetCurrentProcess();

            statistics.AppendLine("Ambiente: DEVELOPMENT\n");

            statistics.AppendLine($"Servidores conectados: {client.Guilds.Count}");
            statistics.AppendLine($"Usuários em cache: {client.Guilds.Values.Sum(g => g.MemberCount)}");
            //statistics.AppendLine($"Shards: {client.ShardCount}");
            //statistics.AppendLine($"Shard ID: {client.ShardId}");

            statistics.AppendLine($"Memória usada: {process.WorkingSet64 / (1024 * 1024)}mb");
            statistics.AppendLine($"Memória livre: {GC.GetTotalMemory(false) / (1024 * 1024)}mb");
            statistics.AppendLine($"Uso da CPU: {GetCpuUsage()}");

            return statistics.ToString();
        }
        private static double GetCpuUsage()
        {
            var process = Process.GetCurrentProcess();
            var totalCpuTime = process.TotalProcessorTime.TotalMilliseconds;
            var uptime = (DateTime.Now - process.StartTime).TotalMilliseconds;
            var cpuUsage = (totalCpuTime / uptime) * 100;
            return cpuUsage;
        }

        //others
    }
}

