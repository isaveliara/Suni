using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public class StartPrefixCommandsGroup : BaseCommandModule
        {
            [Command("quiz")] [Cooldown(1, 60, CooldownBucketType.Channel)] //set cooldown to avoid rate-limit
            public async Task PREFIXCommandStartQuiz(CommandContext ctx, [Option("theme","Tema para jogar")] string theme = "default")
            {
                //trying to find the theme
                var t = await TryFindTheme(theme);
                if (t == null){
                    await ctx.RespondAsync($"Não foi possível encontrar o tema '{theme}' ou falha ao conectar com a api! :x:");
                    return;
                }
                await ctx.Channel.SendMessageAsync($"Começando minigame com o tema {theme}..");

                Dictionary<ulong, (int, int)> usersPoints = new Dictionary<ulong, (int, int)>();

                int numQuestions = -1; //infinite
                int attempts = t.Attempts;//confg
                bool revealAnswerOnFail = t.RevealAnswerOnFail; ////await ctx.RespondAsync($"A resposta correta era: **{QuizquestionData.Answers[0]}**");
                (int rate, int rateVariance) = (t.Rate, t.RateVariance);
                Random random = new Random();

                //-1 makes questions infinite
                for (int x = 0; x != numQuestions; x++)
                {
                    int tryingGetResponse = 1;
                    QuizQuestionData QuizquestionData;
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
                    
                    await ctx.RespondAsync(embed); //message with question

                    for (int y = 0; y !=  attempts; y++)//-1
                    {
                        //the main function
                        var (userResponse, whoResponder) = await GetUserResponseWithCountdown(ctx, rate, rateVariance, QuizquestionData.Answers);
                        if (userResponse == null)
                        {
                            //debug
                            await ctx.Channel.SendMessageAsync($"Esperado as respostas '{string.Join(" | ",QuizquestionData.Answers)}'."); //only for debug
                            if (attempts == -1)
                            {
                                await ctx.Channel.SendMessageAsync($"Sem respostas. Partida finalizada!");
                                return;
                            }
                            continue;
                        }
                        
                        if (usersPoints.ContainsKey(whoResponder)){
                            var has = usersPoints[whoResponder];
                            usersPoints[whoResponder] = (has.Item1 + QuizquestionData.Worth, has.Item2+1);
                        }
                        else
                            usersPoints[whoResponder] = (QuizquestionData.Worth, 1);

                        //usersPoints[whoResponder] = usersPoints.ContainsKey(whoResponder)
                        //    ? usersPoints[whoResponder] + QuizquestionData.Worth //true
                        //    : QuizquestionData.Worth; //false
                        

                        var scoreBoard = new StringBuilder();
                        var top10 = usersPoints
                            .OrderByDescending(us => us.Value)
                            .Take(10);
                        
                        foreach (var u in top10) scoreBoard.AppendLine($"<@{u.Key}> : **{u.Value.Item1}** pontos com **{u.Value.Item2}** respostas");
                        
                        await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithTitle("Placar")
                                .WithDescription(scoreBoard.ToString())
                            )); //sends scoreboard
                        //sends time for next question
                        await ctx.Channel.SendMessageAsync($"{QuizquestionData.Response.Replace("&{getanswer}", userResponse)}\n:small_blue_diamond: Próxima pergunta em: **{(rate + new Random().Next(-rateVariance, rateVariance))>>2} segundos**");
                        break;
                    }
                    await Task.Delay((rate + new Random().Next(-rateVariance, rateVariance)) * 500);
                }
            }

            private async Task<ThemeData> TryFindTheme(string theme)
            {
                string baseUrl = new SunBot.DotenvItems().BaseUrlApi;
                string apiUrl = $"{baseUrl}/quiz/theme/{theme}";

                try
                {
                    var client = new RestClient(apiUrl);
                    var request = new RestRequest();
                    var response = await client.ExecuteAsync(request);
                    Console.WriteLine(response.Content);//

                    if (response.IsSuccessful)
                    {
                        var themeData = new QuizThemeData(JObject.Parse(response.Content));
                        return new ThemeData
                        {
                            NumQuestions = themeData.NumQuestions,
                            RevealAnswerOnFail = themeData.RevealAnswerOnFail,
                            Attempts = themeData.Attempts,
                            NumWins = themeData.NumWins,
                            Rate = themeData.Rate,
                            RateVariance = themeData.RateVariance
                        };
                    }
                    else Console.WriteLine($"Erro: {response.StatusCode} - {response.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"failed to get the theme:\n{ex}");
                }
                return null;
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
                    Console.WriteLine(response.Content);//

                    if (response.IsSuccessful)
                    {
                        var quizData = new QuizData(JObject.Parse(response.Content));
                        return new QuizQuestionData
                        {
                            Build = new BuildQuizEmbedData
                            {
                                Title = quizData.Title,
                                Description = quizData.Description,
                                Color = quizData.Color,
                                Footer = quizData.Footer
                            },
                            Response = quizData.ResponseText,
                            Answers = quizData.Answers.ToObject<List<string>>(),
                            Worth = quizData.Worth
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
                userResponse = userResponse.ToLower();
                foreach (var answer in validAnswers)
                    if (Regex.IsMatch(userResponse, answer.ToLower(), RegexOptions.IgnoreCase))
                        return true;
                
                return false;
            }   

            private static async Task<(string, ulong)> GetUserResponseWithCountdown(CommandContext ctx, int rate, int rateVariance, List<string> QuizquestionData)
            {
                int seconds = rate + new Random().Next(-rateVariance, rateVariance);
                int interval = 10; //10s
                ulong returnWhoResponder = 0;

                //init message
                var countdownMessage = await ctx.Channel.SendMessageAsync($"{seconds} segundos restantes...");

                var interactivity = ctx.Client.GetInteractivity();
                DateTime endTime = DateTime.Now.AddSeconds(seconds);
                string userResponse = null;

                while (DateTime.Now < endTime)
                {
                    var remainingTime = (endTime - DateTime.Now).TotalSeconds;

                    var userMessageTask = interactivity.WaitForMessageAsync(
                        x => x.Channel.Id == ctx.Channel.Id
                            && IsCorrectAnswer(x.Content, QuizquestionData),

                        TimeSpan.FromSeconds(Math.Min(interval, remainingTime))
                    );

                    var result = await Task.WhenAny(userMessageTask, Task.Delay(interval * 1000));
                    if (userMessageTask == result && userMessageTask.Result.Result != null)
                    {
                        userResponse = userMessageTask.Result.Result.Content;
                        returnWhoResponder = userMessageTask.Result.Result.Author.Id;
                        break;
                    }

                    remainingTime = (endTime - DateTime.Now).TotalSeconds;
                    if (remainingTime > 0)
                    {
                        await countdownMessage.ModifyAsync($"{Math.Ceiling(remainingTime)} segundos restantes para responder...");
                    }
                }
                if (userResponse == null)
                {
                    await ctx.Channel.SendMessageAsync("Tempo esgotado! :x:");
                }
                return (userResponse, returnWhoResponder);
            }
        }
    }
    internal class QuizThemeData
    {
        internal JObject ThemeData { get; private set; }
        public QuizThemeData(JObject data)
        {
            ThemeData = (JObject)data["response"]["quiz_info"];
        }
        internal int NumQuestions => (int)ThemeData["numQuestions"];
        internal bool RevealAnswerOnFail => (bool)ThemeData["revealAnswerOnFail"];
        internal int Attempts => (int)ThemeData["chances"];
        internal int NumWins => (int)ThemeData["numWins"];
        internal int Rate => (int)ThemeData["rate"];
        internal int RateVariance => (int)ThemeData["rateVariance"];
    }

    internal class ThemeData
    {
        internal int NumQuestions { get; set; }
        internal bool RevealAnswerOnFail { get; set; }
        internal int Attempts { get; set; }
        internal int NumWins { get; set; }
        internal int Rate { get; set; }
        internal int RateVariance { get; set; }
    }

    internal class QuizData
    {
        internal JObject QuestionData { get; private set; }
        
        public QuizData(JObject data)
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
