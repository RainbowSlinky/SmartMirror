using System;
using SmartMirror.Messenger_Notification.Google;
using SmartMirror.Common;
using System.Collections.Generic;
using Google.Apis.Gmail.v1.Data;
using System.Collections.ObjectModel;

namespace SmartMirror
{
    public class MainPage_ViewModel:ViewModelBase
    {
        
        public Gmail_ViewModel Gmail_Module { get; set; }
        public Content_ViewModel GmailContent_Module { get; set; }
        private Gmail_View gmail_View;
        Content_View gmailContetnt_View;

        public MainPage_ViewModel()
        {
            
            Gmail_Module = new Gmail_ViewModel();
            GmailContent_Module = new Content_ViewModel();
            Gmail_Module.OnListChanged += Gmail_ViewModel_ListChanged;
            Gmail_Module.OnOpenEmailRequest += Gmail_ViewModel_OpenEmail;
            gmail_View = new Gmail_View();
            gmailContetnt_View = new Content_View();
            gmail_View.DataContext = Gmail_Module;
            
        }

        public void Gmail_ViewModel_ListChanged(List<GmailMessage> messageList)
        {
            gmailContetnt_View.DataContext = GmailContent_Module;
            GmailContent_Module.MessageList = new ObservableCollection<GmailMessage>(messageList);
        }

        public void Gmail_ViewModel_OpenEmail(GmailMessage gmailMessage)
        {
            gmailContetnt_View.DataContext = GmailContent_Module;
            GmailContent_Module.GmailMessage = gmailMessage;
        }
    }
}
