using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace iSpeakXamarin.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("http://www.ispeakgroup.com"));
            OpenPrivacyPoliciesCommand = new Command(async () => await Browser.OpenAsync("http://www.ispeakgroup.com/PrivacyPolicies/Index"));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand OpenPrivacyPoliciesCommand { get; }
    }
}