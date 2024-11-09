using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;


namespace Sun.HandlerFunctions{
    public class ErroredSlashFunctions
    {
        public static async Task SlashCommandsErrored_Handler(SlashCommandsExtension sender, SlashCommandErrorEventArgs e){
            await e.Context.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AsEphemeral(true)
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Magenta)
                    .WithTitle("Exception")
                    .WithDescription($"Erro inesperado: {e.Exception}")));
        }

        internal static async Task MenuContextCommandsErrored_Handler(SlashCommandsExtension sender, ContextMenuErrorEventArgs e)
        {
            await e.Context.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AsEphemeral(true)
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Magenta)
                    .WithTitle("Exception")
                    .WithDescription($"Erro inesperado: {e.Exception}")));
        }
    }
    public class ErroredFunctions{

        public static async Task CommandsErrored_Handler(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            string erro_falar = e.Exception.ToString();
            string lang = "pt";
            string say;
            

            Console.WriteLine(e.Exception);

            switch (e.Exception){
                case ArgumentNullException ex:
                    say = $"Nem todos os argumentos necessários para o comando foram passados! {ex.Message}, {ex.ParamName}";
                    break;
                case ArgumentException ex:
                    say = "Argumentos inválidos! :x:";
                    break;
                case UnauthorizedException ex:
                    say = "Sem permissões suficientes para executar o comando!";
                    break;
                case ChecksFailedException ex:
                    say = FailedError(ex, e, lang);
                    break;
                case TimeoutException:
                    say = "Tempo esgotado!";
                    break;
                default: //Else
                    say = $"Erro inesperado:\n```cs\n{erro_falar}\n```";
                    break;
            }
            await e.Context.RespondAsync($"{e.Context.User.Mention} | {say}");
        }

        public static string FailedError(ChecksFailedException erro, CommandErrorEventArgs e, string lang)
        {
            foreach (var falha in erro.FailedChecks)
            {
                switch (falha)
                {
                    case RequirePermissionsAttribute ex:
                        return $"Você não possui as permissões necessárias! ({string.Join(',', ex.Permissions)})";
                    case RequireGuildAttribute:
                        return $"Este comando só pode ser executado em servidores!";
                    case RequireDirectMessageAttribute:
                        return $"Este comando só pode ser executado em mensagem direta!";
                    case RequireOwnerAttribute:
                        return "sem chances de você executar esse comando!";
                    case CooldownAttribute:
                        string timeLeft = "";
                        foreach (var check in erro.FailedChecks)
                        {
                            var coolDown = (CooldownAttribute)check;
                            timeLeft = coolDown.GetRemainingCooldown(e.Context).ToString(@"hh\:mm\:ss");
                        }
                        return $"Aguarde {timeLeft} segundos para poder usar esta ação novamente! ❌";
                    case RequireBotPermissionsAttribute:
                        return "Não tenho as permissões necessárias para executar tal comando!";
                    default:
                        return $"ops! Ocorreu um erro não registrado em meu sistema! Tente utilizar este comando mais tarde..\nDetalhes: ||{falha}||";
                }
            }
            return "unknown error.";
        }
    }
}