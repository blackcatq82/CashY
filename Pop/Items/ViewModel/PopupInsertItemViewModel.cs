using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace CashY.Pop.Items.ViewModel
{
    public partial class PopupInsertItemViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        private string item_cate_name;
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

        private readonly IItemsServices itemsServices;
        private readonly IDatabaseServices  databaseServices;
        private readonly IIPermissionServices iPermissionServices;
        private readonly IPopupServices popupServices;
        private readonly ItemsPageViewModel itemsPageViewModel;
        public PopupInsertItemViewModel(IItemsServices itemsServices, IDatabaseServices databaseServices, IIPermissionServices iPermissionServices, IPopupServices popupServices, ItemsPageViewModel itemsPageViewModel)
        {
            this.itemsServices = itemsServices;
            this.databaseServices = databaseServices;
            this.iPermissionServices=iPermissionServices;
            this.popupServices = popupServices;
            this.itemsPageViewModel = itemsPageViewModel;

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
                var result = databaseServices.categories.ToArray();
                if (result != null)
                {
                    Categorys = result;
                    Item_cate_name = Categorys.Where(x=>x.Id == Item_cate).FirstOrDefault().Cate_name;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error Reload Popinsert :{ex}");
            }
        }

        private async Task SelectImageTask()
        {
            try
            {
                bool isAccpeted = await iPermissionServices.GetRequestPermissionPhotos();
                if(!isAccpeted)
                {
                    await popupServices.ShowPopUpMessage("Permission Photos", "Your didnt not accpet to access photos lib", "OK");
                    return;
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
                await popupServices.ShowPopUpMessage("ERROR!", "Item name can't be empty!", "UNDERSTAND");
                return;
            }
            else if (Item_price <= 0.00)
            {
                await popupServices.ShowPopUpMessage("ERROR!", "Item price can't be lower or equle zero!", "UNDERSTAND");
                return;
            }
            else if (string.IsNullOrEmpty(Item_image))
            {
                await popupServices.ShowPopUpMessage("ERROR!", "Item image can't be empty!", "UNDERSTAND");
                return;
            }
            else if (Item_quantity <= 0)
            {
                await popupServices.ShowPopUpMessage("ERROR!", "Item quantity can't be lower or equle zero!", "UNDERSTAND");
                return;
            }
            else if (CateSelect == null)
            {
                await popupServices.ShowPopUpMessage("ERROR!", "Item select category can't be empty or create a new category!", "UNDERSTAND");
                return;
            }
            else if (Item_price_buy <= 0.00)
            {
                await popupServices.ShowPopUpMessage("ERROR!", "Item price buy can't be lower or equle zero!", "UNDERSTAND");
                return;
            }


            string ItemExpirestr = this.Item_expire_date.ToString("yyyy-MM-dd HH:mm:ss");


            var result = await itemsServices.InsertAsync(this.CateSelect.id, this.Item_name, this.Item_price, this.Item_image, this.Item_quantity, ItemExpirestr, this.Item_price_buy);
            if (result.Item1)
            {
                await popupServices.ShowPopUpMessage("OK!", "is insert a new item in database!", "OK");
                await Shell.Current.Navigation.PopAsync(true);
                itemsPageViewModel.Items.Insert(0, result.Item2);
            }
            else
            {
                await popupServices.ShowPopUpMessage("Error API !", $"something wrong happend!", "OK");
            }
        }
    }
}
