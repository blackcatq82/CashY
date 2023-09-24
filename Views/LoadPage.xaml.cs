using CashY.Views.ViewsModel;
namespace CashY.Views;

public partial class LoadPage : ContentPage
{
	private Timer _timer;
	private readonly LoadPageViewModel vm;
    private readonly Login login;
	public LoadPage(LoadPageViewModel vm, Login login)
	{
		InitializeComponent();
        this.vm = vm;
        this.login = login;
		this.BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (vm is LoadPageViewModel viewModel) { if (viewModel == null) return; }
        _timer = new Timer(UpdateHandlerStatus, null, 0, 100);
        await MainThread.InvokeOnMainThreadAsync(vm.reload);
    }

    private async void UpdateHandlerStatus(object state)
    {
        // Update the ViewModel on the UI thread
        await MainThread.InvokeOnMainThreadAsync(vm.UpdateUI);
        if (vm.IsComplated)
        {
            await _timer.DisposeAsync();
            await MainThread.InvokeOnMainThreadAsync(() => { App.Current.MainPage = login; });
        }
    }
}