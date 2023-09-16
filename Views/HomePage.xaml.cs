namespace CashY.Views;
public partial class HomePage : ContentPage
{
    private ViewsModel.HomePageViewModel vm;
	public HomePage(ViewsModel.HomePageViewModel vm)
	{
        this.vm = vm;
        BindingContext = vm;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Run(async () =>
        {
            await Shell.Current.Dispatcher.DispatchAsync(async () =>
            {
                await vm.Reload();  
            });
        });
    }
}