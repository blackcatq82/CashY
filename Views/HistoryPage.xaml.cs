using CashY.Views.ViewsModel;

namespace CashY.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryPageViewModel viewModel;
    public HistoryPage(HistoryPageViewModel viewModel)
	{
		this.viewModel = viewModel;
		this.BindingContext = this.viewModel;
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await MainThread.InvokeOnMainThreadAsync(viewModel.Reload);
    }
}