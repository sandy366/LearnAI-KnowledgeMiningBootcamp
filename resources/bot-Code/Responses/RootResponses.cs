using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace CognitiveSearchBot.Responses
{
    public class RootResponses
    {
        public static async Task ReplyWithGreeting(ITurnContext context)
        {
            // Add a greeting
            await context.SendActivity($"Hi, I'm CognitiveSearchBot!");
        }
        public static async Task ReplyWithHelp(ITurnContext context)
        {
            await context.SendActivity($"I can retrieve cognitive fields from Azure Search.");
        }
        public static async Task ReplyWithResumeTopic(ITurnContext context)
        {
            await context.SendActivity($"What can I do for you?");
        }
        public static async Task ReplyWithConfused(ITurnContext context)
        {
            await context.SendActivity($"I'm sorry, I don't understand.");
        }
    }
}