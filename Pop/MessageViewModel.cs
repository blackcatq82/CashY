using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
namespace CashY.Pop
{
    public partial class MessageViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        string title;
        [ObservableProperty]
        string message;
        [ObservableProperty]
        string buttonTitle;

        public MessageViewModel()
        {
            title = "info";
            message = "here we will show messages box!";
            buttonTitle = "close";
        }
    }
}
