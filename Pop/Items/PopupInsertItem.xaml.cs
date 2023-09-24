using CashY.Pop.Items.ViewModel;
namespace CashY.Pop.Items;
public partial class PopupInsertItem : ContentPage
{
	private PopupInsertItemViewModel vm;

    public PopupInsertItem(PopupInsertItemViewModel vm)
	{
        this.vm = vm;
        BindingContext = vm;
        InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await MainThread.InvokeOnMainThreadAsync(vm.Reload);
    }
}