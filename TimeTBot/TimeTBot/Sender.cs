using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTBot
{
    public static class Sender
    {
        public static TurnContext Context;

        public static async Task Send(string id)
        {
            Context.Activity.Conversation.Id = id;
            await Context.SendActivityAsync("abc");
        }

        public static async Task InfinitySend(string id)
        {
            while (true)
            {
                await Send(id);
                Thread.Sleep(15000);
            }
        }
    }
}
