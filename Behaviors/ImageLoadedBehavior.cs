using CashY.Model.Items;
namespace CashY.Behaviors
{
    //public class ImageLoadedBehavior : Behavior<Image>
    //{
    //    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
    //        nameof(ViewModel),
    //        typeof(Category),
    //        typeof(ImageLoadedBehavior),
    //        null);

    //    public Category ViewModel
    //    {
    //        get => (Category)GetValue(ViewModelProperty);
    //        set => SetValue(ViewModelProperty, value);
    //    }

    //    protected override void OnAttachedTo(Image bindable)
    //    {
    //        base.OnAttachedTo(bindable);
    //        bindable.PropertyChanged += OnImagePropertyChanged;
    //    }

    //    protected override void OnDetachingFrom(Image bindable)
    //    {
    //        base.OnDetachingFrom(bindable);
    //        bindable.PropertyChanged -= OnImagePropertyChanged;
    //    }

    //    private async void OnImagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == "IsLoading" && !((Image)sender).IsLoading)
    //        {
    //            if(ViewModel != null)
    //            {
    //                await ExecuteCommand();
    //            }
    //        }
    //    }

    //    protected async override void OnBindingContextChanged()
    //    {
    //        if (ViewModel != null)
    //        {
    //            await ExecuteCommand();
    //        }
    //        base.OnBindingContextChanged();
    //    }

    //    private async Task ExecuteCommand()
    //    {
    //        await ViewModel.DownloadImageAsync();
    //    }
    //}
}
