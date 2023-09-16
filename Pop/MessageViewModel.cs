using CashY.ViewModels;
using CashY.Core;
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
            title = FORMATS.Title_INFO;
            message = "here we will show messages box!";
            buttonTitle = FORMATS.CLOSE;
        }
    }
}
