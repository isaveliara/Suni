using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("statistics")]
        public async Task PREFIXCommandStatistcs(CommandContext ctx)
        {
            var stat = GetSuniStatistics();
            await ctx.RespondAsync(stat);
        }

        public static string GetSuniStatistics()
        {
            var statistics = new StringBuilder();
            var client = SunBot.Sun.Client;
            var process = Process.GetCurrentProcess();

            statistics.AppendLine("Ambiente: DEVELOPMENT\n");

            statistics.AppendLine($"Servidores conectados: {client.Guilds.Count}");
            statistics.AppendLine($"Usuários em cache: {client.Guilds.Values.Sum(g=> g.MemberCount)}");
            statistics.AppendLine($"Shards: {client.ShardCount}");
            statistics.AppendLine($"Id do Shard: {client.ShardId}\n");

            statistics.AppendLine($"Memória usada: {process.WorkingSet64 / (1024 * 1024)}mb");
            statistics.AppendLine($"Memória livre: {GC.GetTotalMemory(false) / (1024 * 1024)}mb");
            statistics.AppendLine($"Uso da CPU: {GetCpuUsage()}");
            statistics.AppendLine($"");

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
    }
}