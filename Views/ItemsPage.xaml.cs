using CashY.Views.ViewsModel;

namespace CashY.Views;

public partial class ItemsPage : ContentPage
{
    private ItemsPageViewModel vm;

    public ItemsPage(ItemsPageViewModel vm)
	{
		InitializeComponent();
        this.vm = vm;
		this.BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        // reload first time.
        base.OnAppearing();


        // reload items.
        await Task.Run(async () =>
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await vm.ReloadItems();
            });
        });
    }
    private double previousScrollY = 0;
    private bool isScrollingUp = false;
    private bool isScrollingDown = false;
    private CancellationTokenSource scrollActionCancellation = null;

    private async void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        if (vm.IsSearching) return;


        vm.UpdatePageNavigation();
        if (!vm.HasNextPage) return;

        if (!(sender is ScrollView scrollView)) return;

        if (e.ScrollY > previousScrollY)
        {
            // Scrolling down
            isScrollingUp = false;
            isScrollingDown = true;
        }

        previousScrollY = e.ScrollY;

        var scrollSpace = scrollView.ContentSize.Height - scrollView.Height;
        if (scrollSpace < e.ScrollY) return;

        // Cancel any previously scheduled scroll action
        scrollActionCancellation?.Cancel();
        scrollActionCancellation?.Dispose();

        // Delay for a brief moment before taking action
        scrollActionCancellation = new CancellationTokenSource();
        var delayTask = Task.Delay(TimeSpan.FromMilliseconds(500), scrollActionCancellation.Token);

        try
        {
            await delayTask;
            if (isScrollingDown)
            {
                // Execute NextPage if scrolling down
                await this.vm.NextPage();
                await scrollView.ScrollToAsync(0, 0, true);
            }
        }
        catch (TaskCanceledException)
        {
            // Task was canceled due to user scrolling
        }
        finally
        {
            // Reset scrolling flags
            isScrollingUp = false;
            isScrollingDown = false;
        }
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            vm.IsSearching = false;
        }
    }
}