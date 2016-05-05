using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirror.Messenger_Notification.Google;
using System.Text.RegularExpressions;
using SmartMirror.Auxileriers.Speech;

namespace SmartMirror
{
    public class MainPage_ViewModel : ViewModelBase
    {

        public Gmail_ViewModel Gmail_Module { get; set; }
        private SpeechComponent speechPart;

        public MainPage_ViewModel()
        {
            Gmail_Module = new Gmail_ViewModel();
            speechPart = new SpeechComponent();
            speechPart.commandsGenerated += reactOnSpeech;
            speechPart.sessionsExpired += speechSessionExpired;
            speechPart.startSession();
        }

        private void speechSessionExpired()
        {
            //tbd
            speechPart.startSession();
        }

        private void reactOnSpeech(Dictionary<String, List<String>> commands)
        {

            foreach (var command in commands)
            {
                switch (command.Key)
                {
                    case "showMailList": break; //TBD
                    case "showMails": break; //TBD
                    default: break;

                }
                //var messageDialog = new Windows.UI.Popups.MessageDialog(command.Key, "StartAsync Exception");
                //await messageDialog.ShowAsync();
            }

        }
    }
}
