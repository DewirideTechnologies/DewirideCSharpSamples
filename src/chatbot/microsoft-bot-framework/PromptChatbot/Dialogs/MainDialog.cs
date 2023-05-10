// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PromptChatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;
        private readonly string AgePromptDlgId = "AgePromptDialog";

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(AgePromptDlgId, ValidateAgeAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            AddDialog(new DateResolverDialog());

            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                AgeStepAsync,
                JoingDateStepAsync,
                LanguageStepAsync,
                ProfilePhotoStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is your name?"), RetryPrompt = MessageFactory.Text("Please enter your full name.") }, cancellationToken);
        }

        private async Task<bool> ValidateAgeAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var age = promptContext.Recognized.Value;

                // check whether age is a number
                //Regex regex = new Regex(@"^[0-9]+$");
                //if (!regex.IsMatch(age.ToString()))
                //{
                //    await promptContext.Context.SendActivityAsync(MessageFactory.Text("Please enter a valid age. Age cannot be a text value. Please enter a number."), cancellationToken);
                //    return false;
                //}

                if (age < 18)
                {
                    await promptContext.Context.SendActivityAsync(MessageFactory.Text("You must be at least 18 years old to use this service."), cancellationToken);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                await promptContext.Context.SendActivityAsync(MessageFactory.Text("You must be at least 18 years old to use this service."), cancellationToken);
                return false;
            }
        }

        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(AgePromptDlgId, new PromptOptions { Prompt = MessageFactory.Text("How old are you?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> JoingDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["age"] = (int)stepContext.Result;
            var userDetails = new UserDetails();
            return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), userDetails.JoiningDate,  cancellationToken);
        }

        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["joiningDate"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Choices = ChoiceFactory.ToChoices(new List<string> { "English", "Hindi", "Marathi" }),
                Prompt = MessageFactory.Text("Please select your preferred language."),
                RetryPrompt = MessageFactory.Text("Please select a valid option."),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProfilePhotoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["language"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please upload your profile photo."),
                RetryPrompt = MessageFactory.Text("Please upload a valid image."),
            }, cancellationToken);
        }

        private Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["profilePhoto"] = ((IList<Attachment>)stepContext.Result)[0].ContentUrl;
            
            // download the profile photo attachment and save it to a local folder
            var client = new WebClient();
            client.DownloadFile(new Uri(((IList<Attachment>)stepContext.Result)[0].ContentUrl), "C:\\Users\\Public\\Pictures\\ProfilePhoto.png");

            var msg = $"Please confirm, I have your name as {stepContext.Values["name"]} and your age as {stepContext.Values["age"]}.";
            return stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var userDetails = new UserDetails();
                userDetails.Name = (string)stepContext.Values["name"];
                userDetails.Age = (int)stepContext.Values["age"];
                userDetails.JoiningDate = (string)stepContext.Values["joiningDate"];
                userDetails.Language = (string)stepContext.Values["language"];
                userDetails.ProfilePhoto = (string)stepContext.Values["profilePhoto"];

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for registering with us. You can now use our services."), cancellationToken);
                return await stepContext.EndDialogAsync(userDetails, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for your time. You can always register with us later."), cancellationToken);
                // Restart the main dialog with a different message the second time around
                var promptMessage = "What else can I do for you?";
                return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
            }
        }
    }
}
