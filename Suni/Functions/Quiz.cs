using System.Collections.Generic;
using System.Text.RegularExpressions;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json.Linq;
using RestSharp;
using Suni.Suni.Configuration.Interfaces;

namespace Sun.Functions.Quiz
{
    public class QuizMethods
    {
        internal static async Task<ThemeData> TryFindTheme(string theme, IAppConfig config)
        {
            string baseUrl = config.BaseUrlApi;
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
                        Version = themeData.Version,
                        Requires = themeData.Requires,
                        QuestionsLimite = themeData.QuestionsLimite,
                        RevealAnswerOnFail = themeData.RevealAnswerOnFail,
                        Attempts = themeData.Attempts,
                        PointsForUserWin = themeData.PointsForUserWin,
                        Rate = themeData.Rate,
                        RateVariance = themeData.RateVariance,
                    };
                }
                else Console.WriteLine($"Error: {response.StatusCode} - {response.ErrorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to get the theme:\n{ex}");
            }
            return null;
        }

        internal static async Task<QuizQuestionData> GetQuizQuestion(string theme, IAppConfig config)
        {
            string baseUrl = config.BaseUrlApi;
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
                            File = quizData.File,
                            Footer = quizData.Footer
                        },
                        Response = quizData.ResponseText,
                        Answers = quizData.Answers.ToObject<List<string>>(),
                        Worth = quizData.Worth
                    };
                }
                else
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ErrorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }

            return null;
        }

        internal static bool IsCorrectAnswer(string userResponse, List<string> validAnswers)
        {
            userResponse = userResponse.ToLower();
            foreach (var answer in validAnswers)
                if (Regex.IsMatch(userResponse, answer.ToLower(), RegexOptions.IgnoreCase))
                    return true;

            return false;
        }

        internal static async Task<(string, ulong)> GetUserResponseWithCountdown(CommandContext ctx, int rate, int rateVariance, List<string> QuizquestionData)
        {
            int seconds = rate + new Random().Next(-rateVariance, rateVariance);
            //int interval = 10; //10s
            ulong returnWhoResponder = 0;

            //init message
            var countdownMessage = await ctx.Channel.SendMessageAsync($"{seconds} seconds remaining...");

            var interactivity = ctx.Client.GetInteractivity();
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            string userResponse = null;

            while (DateTime.Now < endTime)
            {
                var remainingTime = (endTime - DateTime.Now).TotalSeconds;
                /*
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
                */

                remainingTime = (endTime - DateTime.Now).TotalSeconds;
                if (remainingTime > 0)
                {
                    await countdownMessage.ModifyAsync($"{Math.Ceiling(remainingTime)} seconds left to answer...");
                }
            }
            if (userResponse == null)
            {
                await ctx.Channel.SendMessageAsync("Time Out! :x:");
            }
            return (userResponse, returnWhoResponder);
        }
    }

    internal class QuizThemeData
    {
        internal JObject ThemeData { get; private set; }
        public QuizThemeData(JObject data)
        {
            ThemeData = (JObject)data["response"]["quiz_info"];
        }

        internal string Version => ThemeData["version"].ToString();
        internal string Requires => ThemeData["requires"].ToString();
        internal int QuestionsLimite => (int)ThemeData["questionsLimite"];
        internal bool RevealAnswerOnFail => (bool)ThemeData["revealAnswerOnFail"];
        internal int Attempts => (int)ThemeData["attempts"];
        internal int PointsForUserWin => (int)ThemeData["pointsForUserWin"];
        internal int Rate => (int)ThemeData["rate"];
        internal int RateVariance => (int)ThemeData["rateVariance"];
    }

    internal class ThemeData
    {
        internal string Requires { get; set; }
        internal string Version { get; set; }
        internal int QuestionsLimite { get; set; }
        internal bool RevealAnswerOnFail { get; set; }
        internal int Attempts { get; set; }
        internal int PointsForUserWin { get; set; }
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
        internal string File => (string)QuestionData["build"]["file"];
        internal string Footer => (string)QuestionData["build"]["footer"];
        internal string ResponseText => (string)QuestionData["response"];
        internal JArray Answers => (JArray)QuestionData["answers"];
        internal int Worth => (int)QuestionData["worth"];
        internal int Index => (int)QuestionData["index"];
        internal string ResponseFile => (string)QuestionData["response_file"];
    }

    //json
    internal class QuizQuestionData
    {
        internal BuildQuizEmbedData Build { get; set; }
        internal int Index { get; set; }
        internal int Worth { get; set; }
        internal string ResponseFile { get; set; }
        internal string Response { get; set; }
        internal List<string> Answers { get; set; }
    }

    internal class BuildQuizEmbedData
    {
        internal string Title { get; set; }
        internal string Description { get; set; }
        internal string Color { get; set; }
        internal string File { get; set; }
        internal string Footer { get; set; }
    }
}

////ignore this
//                    if (numWins != 0 && currentPoints >= numWins)
//                    {
//                        await ctx.Channel.SendMessageAsync("Parabéns! O quiz foi finalizado!");
//                        return;
//                    }
