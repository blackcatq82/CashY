using CashY.Model.Items;

namespace CashY.ContentViews.CategoryViews;

public partial class CategoryViewItem : ContentView
{
	public CategoryViewItem()
	{
		InitializeComponent();
	}

    protected async override void OnBindingContextChanged()
    {
        if(this.BindingContext is Category viewModel)
        {
            await Task.Run(async () =>
            {
                await Dispatcher.DispatchAsync(async () =>
                {
                    await viewModel.DownloadImageAsync();
                });
            });
        }
        base.OnBindingContextChanged();
    }
}