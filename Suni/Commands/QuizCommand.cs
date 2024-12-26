using System.Collections.Generic;
using System.Text;
using Sun.Functions.Quiz;

namespace Sun.Commands;

public class Quiz
{
    [Command("quiz")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
    public static async Task QuizCommand(CommandContext ctx,
        [Parameter("theme")] string theme = "doors")
    {
        //trying to find the theme
        var t = await QuizMethods.TryFindTheme(theme);
        if (t == null){
            await ctx.RespondAsync($"Unable to find theme '{theme}' or failed to connect with api! :x:");
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
                QuizquestionData = await QuizMethods.GetQuizQuestion(theme);
                if (QuizquestionData == null)
                {
                    await ctx.Channel.SendMessageAsync("Failed to make request! :warning:\nRetrying...");
                    await Task.Delay(500);
                }
                else
                    break;
                if (tryingGetResponse == 0)
                {
                    await ctx.Channel.SendMessageAsync("Unable to connect to the api! Try again later :x:");
                    return;
                }
                tryingGetResponse--;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = QuizquestionData.Build.Title,
                Description = QuizquestionData.Build.Description,
                Color = new DiscordColor(QuizquestionData.Build.Color),
                ImageUrl = QuizquestionData.Build.File,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = QuizquestionData.Build.Footer
                }
            };
            
            await ctx.RespondAsync(embed); //message with question

            for (int y = 0; y !=  attempts; y++)//-1
            {
                //the main function
                var (userResponse, whoResponder) = await QuizMethods.GetUserResponseWithCountdown(ctx, rate, rateVariance, QuizquestionData.Answers);
                if (userResponse == null)
                {
                    //debug
                    //await ctx.Channel.SendMessageAsync($"Esperado as respostas '{string.Join(" | ",QuizquestionData.Answers)}'."); //only for debug
                    if (attempts == -1)
                    {
                        await ctx.Channel.SendMessageAsync($"No answers. Game over!");
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
                await ctx.Channel.SendMessageAsync($"{QuizquestionData.Response.Replace("&{answer_provided}", userResponse)}\n:small_blue_diamond: Próxima pergunta em: **{(rate + new Random().Next(-rateVariance, rateVariance))>>2} seconds**");
                break;
            }
            await Task.Delay((rate + new Random().Next(-rateVariance, rateVariance)) * 500);
        }
    }
}