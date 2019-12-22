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
        where TTimeTableEvent : class, ITimeTableEvent, new()
        where TEvent : class, IEvent, new()
        where TUser : class, IUser, new() 
    {
        private static ITurnContext context;

        private Command[] commands;
        private IDbContext<TTimeTableEvent, TEvent, TUser> db;
        private BaseCommandHandler commandHandler;

        public EchoBot(IDbContext<TTimeTableEvent, TEvent, TUser> db, 
            ICacheDictionary<string, Dictionary<string, string>> cacheDictionary)
        {
            this.db = db;

            commands = new Command[]
            {
                new CreateTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db, cacheDictionary),
                new ShowTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db),
                new RemoveTimeTableCommand<TTimeTableEvent, TEvent, TUser>(db, cacheDictionary),
                new GetTopVisitUsersCommand<TTimeTableEvent, TEvent, TUser>(db),
                new NotificatorCommand<TTimeTableEvent, TEvent, TUser>(db, context),
                new ShowAllNotificationsCommand<TTimeTableEvent, TEvent, TUser>(db)
            };

            this.commandHandler = new StandartCommandsHandler(cacheDictionary, commands);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Save context variable
            if (context == null)
                context = turnContext;

            // Register user
            var id = turnContext.Activity.Conversation.Id;
            if (!db.IsUserAdded(id))
            {
                db.TryToAddUser(new TUser() { Id = id, UserName = turnContext.Activity.From.Name });
            }

            // Count visit
            db.AddVisit(id);

            // Send command execution info
            var message = commandHandler.Execute(id, turnContext.Activity.Text);
            if (message != null && message.Length != 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
                return;
            }

            // Write all commands (if previous command is null and user message is not a command)
            foreach (var command in commands)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"{command.Name} ({command.GetDescription()}) \n"), cancellationToken);
            }
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