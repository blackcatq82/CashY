using CashY.Views.ViewsModel;
namespace CashY.Views;
public partial class ItemsPage : ContentPage
{
    private readonly ItemsPageViewModel viewModel;
    public ItemsPage(ItemsPageViewModel vm)
	{
		InitializeComponent();
        // view  model
        this.viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        // reload first time.
        base.OnAppearing();

        if (viewModel is not null) { if (viewModel != null) { if (viewModel.IsLoading) return; } }

        // reload items.
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await viewModel.ReloadItems();
        });

    }

    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            viewModel.IsSearching = false;
            await MainThread.InvokeOnMainThreadAsync(async () => await viewModel.ReloadItems());
        }
    }
}