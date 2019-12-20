// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using TimeTBot;
using TimeTBot.Commands.EventCommands;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot<TTimeTableEvent, TEvent, TUser>: ActivityHandler
        where TTimeTableEvent : class, ITimeTableEvent, new()
        where TEvent : class, IEvent, new()
        where TUser : class, IUser, new() 
    {
        private static ITurnContext context;

        private Command[] commands;
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private ICacheDictionary<string, Dictionary<string, string>> cacheDictionary;

        public EchoBot(IDbContext<TTimeTableEvent, TEvent, TUser> db, ICacheDictionary<string, Dictionary<string, string>> cacheDictionary,
            ITurnContext<IMessageActivity> turnContext)
        {
            this.db = db;
            this.cacheDictionary = cacheDictionary;

            commands = new Command[]
            {
                new CreateTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db, cacheDictionary),
                new ShowTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db),
                new RemoveTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db, cacheDictionary),
                new NotificatorCommand<TTimeTableEvent, TEvent, TUser>(db, turnContext),
                new ShowAllNotificationsCommand<TTimeTableEvent, TEvent, TUser>(db)
            };
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (context == null)
                context = turnContext;

            var id = turnContext.Activity.Conversation.Id;
            if (!db.IsUserAdded(id))
                db.TryToAddUser(new TUser() { Id = id });

            HandleCommands(turnContext, cancellationToken, id);
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

        private void HandleCommands(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string id)
        {
            if (cacheDictionary.ContainsKey(id) && cacheDictionary.GetValue(id).ContainsKey("next"))
            {
                foreach (var command in commands)
                {
                    if (command.Contains(cacheDictionary.GetValue(id)["next"]))
                    {
                        var message = command.Execute(id, turnContext.Activity.Text);
                        if (message != null)
                            turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
                        return;
                    }
                }
            }

            foreach (var command in commands)
            {
                if (command.Contains(turnContext.Activity.Text))
                {
                    var message = command.Execute(id, turnContext.Activity.Text);
                    if (message != null)
                        turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
                    return;
                }
            }

            foreach (var command in commands)
            {
                turnContext.SendActivityAsync(MessageFactory.Text($"{command.Name} ({command.GetDescription()}) \n"), cancellationToken);
            }
        }
    }
}