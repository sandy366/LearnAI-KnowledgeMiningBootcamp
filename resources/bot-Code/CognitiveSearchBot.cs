using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using CognitiveSearchBot.Responses;
using CognitiveSearchBot.Dialogs;
using Microsoft.Azure.Search;
using System.Collections.Generic;

namespace CognitiveSearchBot
{
    public class CognitiveSearchBot : IBot
    {
        private const string RootDialog = "rootDialog";
        private DialogSet _dialogs { get; } = ComposeMainDialog();

        /// <summary>
        /// Every Conversation turn for our bot calls this method. 
        /// </summary>
        /// <param name="context">The current turn context.</param>        
        public async Task OnTurn(ITurnContext context)
        {

            if (context.Activity.Type is ActivityTypes.Message)
            {
                // Get the user and conversation state from the turn context.
                var state = UserState<UserData>.Get(context);
                var conversationInfo = ConversationState<ConversationInfo>.Get(context);

                // Establish dialog state from the conversation state.
                var dc = _dialogs.CreateContext(context, conversationInfo);

                // Continue any current dialog.
                await dc.Continue();
                // Every turn sends a response, so if no response was sent,
                // then there no dialog is currently active.
                if (!context.Responded)
                {
                    // Greet them if we haven't already
                    if (state.Greeted == "not greeted")
                    {
                        await RootResponses.ReplyWithGreeting(context);
                        await RootResponses.ReplyWithHelp(context);
                        state.Greeted = "greeted";
                    }
                    else
                    {
                        await dc.Begin(RootDialog);
                    }
                }
            }
        }

        /// <summary>
        /// Composes a main dialog for our bot.
        /// </summary>
        /// <returns>A new main dialog.</returns>
        private static DialogSet ComposeMainDialog()
        {
            var dialogs = new DialogSet();

            dialogs.Add(RootDialog, new WaterfallStep[]
            {
                // Duplicate the following row if your dialog will have 
                // multiple turns. In this case, we just have one
                async (dc, args, next) =>
                {
                    // Get the state of the conversation 
                    var conversation = ConversationState<ConversationInfo>.Get(dc.Context);
                    // If Regex picks up on anything, store it
                    var recognizedIntents = dc.Context.Services.Get<IRecognizedIntents>();
                    // Based on the recognized intent, direct the conversation
                    switch (recognizedIntents.TopIntent?.Name)
                    {
                            case "search":
                                if (recognizedIntents.TopIntent.Entities.Count > 0)
                                {
                                    var dictionary = new Dictionary<string, object>
                                    {
                                        { "Value" , recognizedIntents.TopIntent.Entities[0].Value  },

                                    };

                                    // Switches to SearchDialog with context.

                                    await dc.Begin(SearchDialog.Id, dictionary);
                                } else{
                                    await dc.Begin(SearchDialog.Id, null);
                                }
                                break;
                            case "help":
                                // show help
                                await RootResponses.ReplyWithHelp(dc.Context);
                                break;
                        default:
                                await RootResponses.ReplyWithConfused(dc.Context);
                                break;
                    }
                }

            });

            // Add our child dialogs (in this case just one)
            dialogs.Add(SearchDialog.Id, SearchDialog.Instance);

            return dialogs;
        }
    }
}