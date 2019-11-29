// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using TimeTBot;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot<TTimeTableEvent, TEvent, TUser>: ActivityHandler
        where TTimeTableEvent : class, ITimeTableEvent
        where TEvent : class, IEvent
        where TUser : class, IUser 
    {
        private static ITurnContext context;
        private static IDbContext<TTimeTableEvent, TEvent, TUser> db;

        public EchoBot()
        {
        }

        //private static List<Command> commandList { get; }

        //static EchoBot()
        //{
        //    commandList = new List<Command>() { new AddTimeTableEventCommand() };
        //}

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //if (context == null)
            //    context = turnContext;
            //switch (turnContext.Activity.Text.ToLower())
            //{
            //    case "hi":
            //        await turnContext.SendActivityAsync(MessageFactory.Text($"Hi!"), cancellationToken);
            //        Id = turnContext.Activity.Conversation.Id;
            //        break;
            //    default:
            //        await turnContext.SendActivityAsync(MessageFactory.Text($"Echo (1): {turnContext.Activity.Text}"), cancellationToken);
            //        if (Id != null)
            //        {
            //            turnContext.Activity.Conversation.Id = Id;
            //            await turnContext.SendActivityAsync(MessageFactory.Text($"{turnContext.Activity.Text}"), cancellationToken);
            //            foreach (var user in Db.Users)
            //            {
            //                await turnContext.SendActivityAsync(MessageFactory.Text($"{user.Email} = {user.Password}"), cancellationToken);
            //            }
            //        }
            //        break;
            //}

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}