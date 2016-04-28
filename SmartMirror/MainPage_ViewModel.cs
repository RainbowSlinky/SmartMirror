using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirror.Messenger_Notification.Google;
namespace SmartMirror
{
    public class MainPage_ViewModel:ViewModelBase
    {
        
        public Gmail_ViewModel Gmail_Module { get; set; }

        public MainPage_ViewModel()
        {
            Gmail_Module = new Gmail_ViewModel();
        }
    }
}
