using CashY.Views.ViewsModel;
namespace CashY.Views;
public partial class CategoryPage : ContentPage
{
    private readonly CategoryPageViewModel viewModel;
    public CategoryPage(CategoryPageViewModel vm)
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

        if (viewModel is CategoryPageViewModel vm) { if (vm.IsLoading) return; }
        await MainThread.InvokeOnMainThreadAsync(viewModel.LoadCategorysExecute);
    }
    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            viewModel.IsSearching = false;
            await MainThread.InvokeOnMainThreadAsync(viewModel.LoadCategorysExecute);
        }
    }

    private async void searchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(searchBar.Text))
        {
            viewModel.IsSearching = false;
            await MainThread.InvokeOnMainThreadAsync(viewModel.LoadCategorysExecute);
        }
    }
}