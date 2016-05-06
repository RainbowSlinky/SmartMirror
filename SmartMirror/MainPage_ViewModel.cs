using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirror.Messenger_Notification.Google;
using System.Text.RegularExpressions;
using SmartMirror.Auxileriers.Speech;
using Windows.UI.Core;
using Windows.Media.SpeechRecognition;

namespace SmartMirror
{
    public class MainPage_ViewModel : ViewModelBase
    {

        public Gmail_ViewModel Gmail_Module { get; set; }
        private SpeechComponent speechPart;
        private CoreDispatcher dispatcher;
        public Windows.UI.Xaml.Media.Brush indicator { get; set; }
        public MainPage_ViewModel()
        {

            indicator = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
            Gmail_Module = new Gmail_ViewModel();
            speechPart = new SpeechComponent();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            speechPart.commandsGenerated += reactOnSpeech;
            speechPart.sessionsExpired += speechSessionExpired;
            speechPart.sessionStateChanged += speechStateChanged;
            speechPart.startSession();
            indicator = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
        }

        private async void speechSessionExpired()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>{ indicator = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red); });
            //tbd
            speechPart.startSession();

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { indicator = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green); });
        }

        private void speechStateChanged(SpeechRecognizerState newState)
        {
           //nothing as of yet
        }

        private async void  reactOnSpeech(string command,string param)
        {

             switch (command)
                {
                    case "showMailList": break; //TBD
                    case "showMails": break; //TBD
                    case "closeCalender":break; //TBD
                    case "openCalender":break; //TBD
                case "openNews":
                case "closeNews":
                case "openSpecificNews":
                case "closeSpecificNews":
                    default: break;

                }
              await  dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(command+" parameter was: " +param, "Command detected");
                    await messageDialog.ShowAsync();
                });
        }
    }
}
