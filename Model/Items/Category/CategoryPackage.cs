using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace CashY.Model.Items
{
    public partial class CategoryPackage : NewBaseViewModel
    {
        [ObservableProperty()]
        private int status;

        private Category category;
        [JsonProperty("data")]
        public Category Category
        {
            get => category;
            set => SetProperty(ref category, value);
        }

        private Category[] categorys;
        [JsonProperty("getall")]
        public Category[] Categorys
        {
            get => categorys;
            set => SetProperty(ref categorys, value);
        }

        [ObservableProperty()]
        private string message;

        [ObservableProperty()]
        private string error;
    }
}
