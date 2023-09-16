using CommunityToolkit.Mvvm.ComponentModel;
namespace CashY.ViewModels
{
    public partial class NewBaseViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;
    }
}
