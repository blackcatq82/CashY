using CashY.Views.ViewsModel;
namespace CashY.Views;
public partial class PaymentPage : ContentPage
{
    private readonly PaymentPageViewModel vm;
    public PaymentPage(PaymentPageViewModel vm)
	{
		this.vm = vm;
		this.BindingContext = vm;
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        // reload first time.
        base.OnAppearing();

        // reload items.
        await MainThread.InvokeOnMainThreadAsync(vm.Reload);
    }
}