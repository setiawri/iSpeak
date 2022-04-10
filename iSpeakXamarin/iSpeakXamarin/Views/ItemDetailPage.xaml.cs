using iSpeakXamarin.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace iSpeakXamarin.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}