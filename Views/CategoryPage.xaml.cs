using CashY.Views.ViewsModel;
namespace CashY.Views;
public partial class CategoryPage : ContentPage
{
    private CategoryPageViewModel viewModel { get; set; }
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


        // reload category.
        await Task.Run(async () =>
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await viewModel.LoadCategorys();
            });
        });
    }
    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            viewModel.IsSearching = false;
        }
    }
}