using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartMirror.Messenger_Notification.Google
{
    public class Gmail_ViewModel:ViewModelBase
    {
        public string Message_Count { get; set; }
        public ImageSource Gmail_Icon { get; set; }
        private UserCredential credential;

        public  Gmail_ViewModel()
        {
            Message_Count = "12";
            BitmapImage bitmap_Gmail = new BitmapImage();
            bitmap_Gmail.UriSource = new System.Uri("ms-appx:///Messenger_Notification/Google/gmail-icon.png");
            Gmail_Icon = bitmap_Gmail;
            //createOAuthCredentials();
        }

        public async void createOAuthCredentials()
        {
            var dialog = new MessageDialog("");
            string[] Scopes = { GmailService.Scope.MailGoogleCom };
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new Uri("ms-appx:///Messenger_Notification/Google/client_secret.json"),
            Scopes, 
            "user", 
            CancellationToken.None).Result;

            if (credential.Token.IsExpired(credential.Flow.Clock))
            {
                dialog.Content = "Token has expired";
                await dialog.ShowAsync();
                if(credential.RefreshTokenAsync(CancellationToken.None).Result)
                {
                    dialog.Content = "Token is refreshed";
                    await dialog.ShowAsync();
                }
                else
                {
                    dialog.Content = "Couldn't refresh token";
                    await dialog.ShowAsync();
                }
            }
            else
            {
                dialog.Content = "Token is valid";
                await dialog.ShowAsync();
            }

            Oauth2Service oauthService = new Oauth2Service(
            new BaseClientService.Initializer() { HttpClientInitializer = credential });

        }
    }
}
