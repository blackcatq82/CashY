using CashY.ViewModels;
namespace CashY;
public partial class Login : ContentPage
{
    private readonly LoginViewModel vm;
    public Login(LoginViewModel vm)
	{
        this.vm = vm;
        BindingContext = this.vm;
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // reload items.
        await MainThread.InvokeOnMainThreadAsync(vm.Reload);
    }
}


