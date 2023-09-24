namespace CashY.Views;
public partial class HomePage : ContentPage
{
    private readonly ViewsModel.HomePageViewModel vm;
	public HomePage(ViewsModel.HomePageViewModel vm)
	{
        this.vm = vm;
        BindingContext = vm;
        InitializeComponent();
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        // reload items.
        await MainThread.InvokeOnMainThreadAsync(vm.Reload);
    }
}