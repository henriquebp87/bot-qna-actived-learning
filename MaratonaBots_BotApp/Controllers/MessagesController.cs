﻿using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;

namespace MaratonaBots_BotApp
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;

            if (activity.Type == ActivityTypes.Message)
            {
                //para Dialog default
                //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog()); 

                //para FAQ Bot
                //await Conversation.SendAsync(activity, () => new Dialogs.QnaDialog()); 

                //para LUIS Bot
                var luisAttributes = new LuisModelAttribute(
                    ConfigurationManager.AppSettings["LuisId"],
                    ConfigurationManager.AppSettings["LuisSubscriptionKey"]
                    );
                var luisService = new LuisService(luisAttributes);
                await Conversation.SendAsync(activity, () => new Dialogs.CotacaoDialog(luisService));

                //para FormFlow
                //await this.SendConversation(activity); 
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && activity.MembersAdded.Any())
                {
                    foreach (var member in activity.MembersAdded)
                    {
                        if (member.Id != activity.Recipient.Id)
                        {
                            //para FormFlow
                            //await this.SendConversation(activity);

                            HandleSystemMessage(activity);
                        }
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task SendConversation(Activity activity)
        {
            await Conversation.SendAsync(activity,
                () => Chain.From(
                    () => FormDialog.FromForm(
                        () => Formulario.Pedido.BuildForm(), FormOptions.PromptFieldsWithValues)));
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}