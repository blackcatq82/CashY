using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
namespace CashY.Model.Items.Items
{
    public partial class ItemPackage : NewBaseViewModel
    {
        [ObservableProperty()]
        private int status;

        private MyItem item;
        [JsonProperty("data")]
        public MyItem Item
        {
            get => item;
            set => SetProperty(ref item, value);
        }

        private MyItem[] items;
        [JsonProperty("getall")]
        public MyItem[] Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        private string error;
        public string Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

    }
}
