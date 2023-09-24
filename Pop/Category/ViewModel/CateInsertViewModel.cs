using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
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


        public readonly CategoryPageViewModel viewModel;
        private readonly IIPermissionServices  iPermissionServices;
        private readonly ICategoryServices _categoriesServices;
        private readonly IPopupServices popupServices;
        public CateInsert cateInsert { get => _cateInsert; set => _cateInsert = value; }
        private CateInsert _cateInsert;
        public CateInsertViewModel(CategoryPageViewModel viewModel, IIPermissionServices iPermissionServices, ICategoryServices _categoriesServices, IPopupServices popupServices)
        {
            this.iPermissionServices = iPermissionServices;
            this._categoriesServices = _categoriesServices;
            this.viewModel = viewModel;
            this.popupServices=popupServices;

            ImportImage = new AsyncRelayCommand(Import);
            Save = new AsyncRelayCommand(SaveItem);

            imageCateDisplay = ImageSource.FromFile("splash.png");
        }

        public async Task Import()
        {
            try
            {
                await iPermissionServices.GetRequestPermissionPhotos();

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
                await cateInsert.CloseAsync();
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        public async Task SaveItem()
        {
            popupServices.Show();
            if (string.IsNullOrEmpty(Cate_Name) || string.IsNullOrEmpty(Cate_Image))
            {
                popupServices.Close();
                await cateInsert.CloseAsync();
                await Shell.Current.DisplayAlert("INFO", "Input category name or image selected can't be nullable or empty!", "OK");
                return;
            }

            var result = await _categoriesServices.InsertAsync(Cate_Name, Cate_Image);
            if (result.Item1)
            {
                viewModel.Categorys.Insert(0, result.Item2);
                popupServices.Close();
                await cateInsert.CloseAsync();
                await Shell.Current.DisplayAlert("INFO", "Has insert a new row.", "OK");
                return;
            }
            else
            {
                popupServices.Close();
                await cateInsert.CloseAsync();
                await Shell.Current.DisplayAlert("Error", "Error: insert a new row got error!!", "OK!");
                return;
            }
        }
    }
}
