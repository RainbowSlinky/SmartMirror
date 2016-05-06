using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirror.Messenger_Notification.Google;
using System.Text.RegularExpressions;
using SmartMirror.Auxileriers.Speech;
using Windows.UI.Core;

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

        private async void  reactOnSpeech(Dictionary<SupportedCommands, List<String>> commands)
        {

            foreach (var command in commands)
            {
                switch (command.Key)
                {
                    case SupportedCommands.showMailList: break; //TBD
                    case SupportedCommands.showMails: break; //TBD
                    case SupportedCommands.closeCalender:break; //TBD
                    case SupportedCommands.openCalender:break; //TBD
                    default: break;

                }
              await  dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(Enum.GetName(typeof(SupportedCommands),command.Key), "Command detected");
                    await messageDialog.ShowAsync();
                });
               
            }

        }
    }
}
