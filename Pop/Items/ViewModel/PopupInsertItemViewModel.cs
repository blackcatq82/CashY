using CashY.Model.Items;
using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashY.Pop.Items.ViewModel
{
    public partial class PopupInsertItemViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        private int item_cate;
        [ObservableProperty]
        private string item_name;
        [ObservableProperty]
        private double item_price;
        [ObservableProperty]
        private string item_image;
        [ObservableProperty]
        private int item_quantity;
        [ObservableProperty]
        private DateTime item_expire_date;
        [ObservableProperty]
        private double item_price_buy;

        [ObservableProperty]
        private ImageSource myImageSource;



        [ObservableProperty]
        private Model.Items.Category cateSelect;

        [ObservableProperty]
        private Model.Items.Category[] categorys;
        public IAsyncRelayCommand Insert { get; set; }
        public IAsyncRelayCommand SelectImage { get; set; }

        private IItemsServices itemsServices;
        private ICategoryServices categoriesServices;
        public PopupInsertItemViewModel(IItemsServices itemsServices, ICategoryServices categoriesServices)
        {
            this.itemsServices = itemsServices;
            this.categoriesServices = categoriesServices;

            SelectImage = new AsyncRelayCommand(SelectImageTask);
            Insert = new AsyncRelayCommand(InsertTask);

            myImageSource = ImageSource.FromFile("splash.png");

            item_expire_date = DateTime.Now;
            item_expire_date.AddYears(1);
            
        }

        public async Task Reload()
        {
            try
            {
                var result = await categoriesServices.GetCategoriesAsync();
                if (result != null)
                {
                    Categorys = result;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SelectImageTask()
        {
            try
            {
                var platform = DeviceInfo.Platform;

                if (platform == DevicePlatform.iOS || platform == DevicePlatform.MacCatalyst || platform == DevicePlatform.macOS || platform == DevicePlatform.tvOS)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.Photos>();
                        if (status != PermissionStatus.Granted)
                        {
                            // Handle permission denied
                            _ = App.Current.MainPage.DisplayAlert("Error", "Handle permission denied!", "OK");
                        }
                    }
                }
                var mediaOptions = new MediaPickerOptions
                {
                    Title = "Select Image"
                };

                var selectedImage = await MediaPicker.PickPhotoAsync(mediaOptions);

                if (selectedImage != null)
                {
                    // Handle the selected image here (e.g., display it in an image view).
                    // selectedImage.Path contains the path to the selected image.
                    Item_image = selectedImage.FullPath;
                    MyImageSource = ImageSource.FromFile(Item_image);
                }
            }
            catch (Exception ex)
            {
                _ = App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async Task InsertTask()
        {
            if(string.IsNullOrEmpty(Item_name))
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item name can't be empty!", "UNDERSTAND");
                return;
            }
            else if (Item_price <= 0.00)
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item price can't be lower or equle zero!", "UNDERSTAND");
                return;
            }
            else if (string.IsNullOrEmpty(Item_image))
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item image can't be empty!", "UNDERSTAND");
                return;
            }
            else if (Item_quantity <= 0)
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item quantity can't be lower or equle zero!", "UNDERSTAND");
                return;
            }
            else if (CateSelect == null)
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item select category can't be empty or create a new category!", "UNDERSTAND");
                return;
            }
            else if (Item_price_buy <= 0.00)
            {
                await Shell.Current.DisplayAlert("ERROR!", "Item price buy can't be lower or equle zero!", "UNDERSTAND");
                return;
            }


            string ItemExpirestr = this.Item_expire_date.ToString("yyyy-MM-dd HH:mm:ss");


            var result = await itemsServices.InsertAsync(this.CateSelect.id, this.Item_name, this.Item_price, this.Item_image, this.Item_quantity, ItemExpirestr, this.Item_price_buy);
            if(result != null)
            {
                if (string.IsNullOrEmpty(result.Error))
                {
                    await Shell.Current.DisplayAlert("OK!", "is insert a new item in database!", "OK");
                    await Shell.Current.Navigation.PopAsync(true);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error API !", $"{result.Error}!", "OK");
                }
            }
        }
    }
}
