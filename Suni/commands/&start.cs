using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace SunPrefixCommands
{
    public partial class GameCommands : BaseCommandModule
    {
        [Group("start")]
        public class StartPrefixCommandGroup : BaseCommandModule
        {
            [Command("quiz")] [Cooldown(1, 60, CooldownBucketType.Guild)] //set cooldown to avoid rate-limit
            public async Task PREFIXCommandStartQuiz(CommandContext ctx, [Option("theme","Tema para jogar")] string theme = "default")
            {
                int currentPoints, numWins = currentPoints = 0;
                int numQuestions = -1;
                int attempts = 1;
                ////bool revealAnswerOnFail = false; //maybe set this on api ////await ctx.RespondAsync($"A resposta correta era: **{QuizquestionData.Answers[0]}**");
                (int rate, int rateVariance) = (10, 5);
                Random random = new Random();

                //-1 makes infinite
                for (int x = 0; x != numQuestions; x++)
                {
                    int tryingGetResponse = 1;
                    QuizQuestionData QuizquestionData; //define
                    while (true)
                    {
                        QuizquestionData = await GetQuizQuestion(theme);
                        if (QuizquestionData == null)
                        {
                            await ctx.Channel.SendMessageAsync("Falha ao fazer request! :warning:\nRetrying...");
                            await Task.Delay(500);
                        }
                        else
                            break;
                        if (tryingGetResponse == 0)
                        {
                            await ctx.Channel.SendMessageAsync("Não foi possível fazer conexão com a api! :x:");
                            return;
                        }
                        tryingGetResponse--;
                    }

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = QuizquestionData.Build.Title,
                        Description = QuizquestionData.Build.Description,
                        Color = new DiscordColor(QuizquestionData.Build.Color),
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = QuizquestionData.Build.Footer
                        }
                    };

                    await ctx.RespondAsync(embed);
                    ////bool correctAnswer = false;

                    for (int y = 0; y !=  attempts; y++)
                    {
                        var userResponse = await GetUserResponse(ctx, rate, rateVariance);
                        if (userResponse == null)
                        {
                            await ctx.Channel.SendMessageAsync("Sem respostas. Partida finalizada!");  return;
                        }

                        if (IsCorrectAnswer(userResponse, QuizquestionData.Answers))
                        {
                            await ctx.Channel.SendMessageAsync(QuizquestionData.Response.Replace("&{getanswer}", userResponse));
                            currentPoints += QuizquestionData.Worth;
                            ////correctAnswer = true;
                            break;
                        }
                        int timewait = rate + random.Next(-rateVariance, rateVariance);
                        await ctx.Channel.SendMessageAsync($"Próxima pergunta em {timewait} segundos!");
                        await Task.Delay(timewait * 1000);
                    }
                }
                Console.WriteLine("debug");//debug
            }

            private static async Task<QuizQuestionData> GetQuizQuestion(string theme)
            {
                string baseUrl = new SunBot.DotenvItems().BaseUrlApi;
                string apiUrl = $"{baseUrl}/quiz/question/{theme}";

                try
                {
                    var client = new RestClient(apiUrl);
                    var request = new RestRequest();
                    var response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful)
                    {
                        var ados = new Ados(JObject.Parse(response.Content));
                        return new QuizQuestionData
                        {
                            Build = new BuildQuizEmbedData
                            {
                                Title = ados.Title,
                                Description = ados.Description,
                                Color = ados.Color,
                                Footer = ados.Footer
                            },
                            Response = ados.ResponseText,
                            Answers = ados.Answers.ToObject<List<string>>(),
                            Worth = ados.Worth
                        };
                    }
                    else
                        Console.WriteLine($"Erro: {response.StatusCode} - {response.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exceção: {ex.Message}");
                }

                return null;
            }

            private static bool IsCorrectAnswer(string userResponse, List<string> validAnswers)
            {
                foreach (var answer in validAnswers)
                    if (Regex.IsMatch(userResponse, answer, RegexOptions.IgnoreCase))
                        return true;
                
                return false;
            }

            private static async Task<string> GetUserResponse(CommandContext ctx, int rate, int rateVariance)
            {
                var interactivity = ctx.Client.GetInteractivity();
                var userMessage = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id, TimeSpan.FromSeconds(rate + new Random().Next(-rateVariance, rateVariance)));
                return userMessage.Result?.Content;
            }

        }
    }
    internal class Ados
    {
        internal JObject QuestionData { get; private set; }
        
        public Ados(JObject data)
        {
            QuestionData = (JObject)data["response"];
        }

        internal string Title => (string)QuestionData["build"]["title"];
        internal string Description => (string)QuestionData["build"]["description"];
        internal string Color => (string)QuestionData["build"]["color"];
        internal string Footer => (string)QuestionData["build"]["footer"];
        internal string ResponseText => (string)QuestionData["response"];
        internal JArray Answers => (JArray)QuestionData["answers"];
        internal int Worth => (int)QuestionData["worth"];
    }
    
    //json
    internal class QuizQuestionData
    {
        internal BuildQuizEmbedData Build { get; set; }
        internal string Response { get; set; }
        internal List<string> Answers { get; set; }
        internal int Worth { get; set; }
    }

    internal class BuildQuizEmbedData
    {
        internal string Title { get; set; }
        internal string Description { get; set; }
        internal string Color { get; set; }
        internal string Footer { get; set; }
    }
}

////ignore this
//                    if (numWins != 0 && currentPoints >= numWins)
//                    {
//                        await ctx.Channel.SendMessageAsync("Parabéns! O quiz foi finalizado!");
//                        return;
//                    }
