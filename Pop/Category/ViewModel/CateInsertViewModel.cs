using CashY.Core;
using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashY.Pop.Category.ViewModel
{
    public partial class CateInsertViewModel : NewBaseViewModel
    {

        [ObservableProperty]
        ImageSource imageCateDisplay;

        [ObservableProperty]
        public string cate_Image;
        [ObservableProperty]
        public string cate_Name;

        [ObservableProperty] 
        public string errorMessage;

        public IAsyncRelayCommand ImportImage { get; set; }
        public IAsyncRelayCommand Save { get; set; }

        private BusyIndicator busyIndicator { get; set; }
        public CategoryPageViewModel viewModel { get; set; }
        private IServiceProvider serviceProvider { get; set; }

        private IMyHttpRequests _httpRequests { get; set; }
        private ICategoryServices _categoriesServices { get; set; }

        public CateInsert cateInsert { get => _cateInsert; set => _cateInsert = value; }
        private CateInsert _cateInsert;
        public CateInsertViewModel(CategoryPageViewModel viewModel,  IServiceProvider serviceProvider, IMyHttpRequests _httpRequests, ICategoryServices _categoriesServices)
        {
            this.serviceProvider = serviceProvider;
            this._httpRequests = _httpRequests;
            this._categoriesServices = _categoriesServices;
            this.viewModel = viewModel;

            ImportImage = new AsyncRelayCommand(Import);
            Save = new AsyncRelayCommand(SaveItem);

            imageCateDisplay = ImageSource.FromFile("splash.png");
        }

        public async Task Import()
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
                    Cate_Image = selectedImage.FullPath;
                    ImageCateDisplay = ImageSource.FromFile(Cate_Image);
                }
            }
            catch (Exception ex)
            {
                _ = App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        public async Task SaveItem()
        {
            await ShowBusyIndicator();

            if (string.IsNullOrEmpty(Cate_Name) || string.IsNullOrEmpty(Cate_Image))
            {
                await busyIndicator.CloseAsync();
                 _ = App.Current.MainPage.DisplayAlert("INFO", "Input category name or image selected can't be nullable or empty!", "OK");
                return;
            }
            string namefile = Cate_Name;
            var result_post = await _httpRequests.ImagePostAsync(FORMATS.URL_POST_IMAGES, Cate_Image, namefile);

            if (!result_post)
            {
                await busyIndicator.CloseAsync();
                _ = App.Current.MainPage.DisplayAlert("INFO","Something wrong wiht upload image to websocket try again later..", "OK");
                await cateInsert.CloseAsync();
                return;
            }

            string ex = Path.GetExtension(Cate_Image);

            if (!Cate_Name.Contains(ex))
                Cate_Name = string.Format("{0}{1}", Cate_Name, Path.GetExtension(Cate_Image));

            var result = await _categoriesServices.InsertAsync(Cate_Name.Replace(ex, ""), Cate_Name);
            if (result != null)
            {
                if (result.Status == 0)
                {
                    await busyIndicator.CloseAsync();
                    _ = App.Current.MainPage.DisplayAlert("INFO", "Has insert a new row.", "OK");
                    await viewModel.LoadCategorys();
                    await cateInsert.CloseAsync();
                    return;
                }
                else
                {
                    await busyIndicator.CloseAsync();
                    ErrorMessage = string.Format("Error: {0}", result.Error);
                    _ = App.Current.MainPage.DisplayAlert("", ErrorMessage, "");
                    await cateInsert.CloseAsync();
                    return;
                }
            }
            else
            {
                await busyIndicator.CloseAsync();
                ErrorMessage = string.Format("Error: {0}", "We are didn't respone anything from website...");
                _ = App.Current.MainPage.DisplayAlert("", ErrorMessage, "");
                await cateInsert.CloseAsync();
                return;

            }
        }
        private Task ShowBusyIndicator()
        {
            if(busyIndicator is not null)
                return Task.CompletedTask;

            using (var Scope = serviceProvider.CreateScope())
            {
                busyIndicator = Scope.ServiceProvider.GetService<BusyIndicator>();
                if(busyIndicator is not null)
                {
                    App.Current.MainPage.ShowPopup(busyIndicator);
                }
            }

            return Task.CompletedTask;
        }
    }
}
