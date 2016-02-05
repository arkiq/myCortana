using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;

namespace superGameVoiceServiceProject  // project name
{
    // rename class1 to superGameVoiceService
    public sealed class superGameVoiceService : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        VoiceCommandServiceConnection voiceServiceConection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // inform the system that the background task may continue after the 
            // Run method has completed
            this._deferral = taskInstance.GetDeferral();

            taskInstance.Canceled += TaskInstance_Canceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if( (triggerDetails != null) &&
                (triggerDetails.Name == "superGameVoiceService"))
            {
                try
                {
                    voiceServiceConection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    voiceServiceConection.VoiceCommandCompleted += VoiceServiceConection_VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "highScorer":
                            {
                       
                                sendCompletionMessageForHighScorer();
                                break;
                            }
                        default:
                            {
                                launchAppInForeground();
                                break;
                            }
                    }
                }
                finally 
                {
                    if( this._deferral != null)
                    {
                        // complete the service deferral
                        this._deferral.Complete();
                    }
                }

            } // end if (triggerDetails
        }

        private async void launchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launching superGame";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConection.RequestAppLaunchAsync(response);            
        }

        private async void sendCompletionMessageForHighScorer()
        {
            // longer than 0.5 seconds, then progress report has to be sent
            string progressMessage = "Finding the highest scorer";
            await ShowProgressScreen(progressMessage);

            var userMsg = new VoiceCommandUserMessage();
            userMsg.DisplayMessage = userMsg.SpokenMessage = "The person with the highest score is Damien";

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMsg);
            await voiceServiceConection.ReportSuccessAsync(response);
        }

        private async Task ShowProgressScreen(string progressMessage)
        {
            var userProgressMsg = new VoiceCommandUserMessage();
            userProgressMsg.DisplayMessage = userProgressMsg.SpokenMessage = progressMessage;
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMsg);
            await voiceServiceConection.ReportProgressAsync(response);
        }

        private void VoiceServiceConection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, 
                                                                 VoiceCommandCompletedEventArgs args)
        {
            if( this._deferral != null)
            {
                this._deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this._deferral != null)
            {
                this._deferral.Complete();
            }
        }
    }
}
