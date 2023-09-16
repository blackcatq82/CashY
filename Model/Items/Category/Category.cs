using CashY.Core;
using CashY.Pop;
using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;



namespace CashY.Model.Items
{
    public partial class Category : NewBaseViewModel
    {
        private bool isReloadImage = false;

        [JsonPropertyName("id")]
        public int id;
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [JsonPropertyName("cate_name")]
        public string cate_name;
        public string Cate_name
        {
            get => cate_name;
            set => SetProperty(ref cate_name, value);
        }

        [JsonPropertyName("cate_image")]
        public string cate_image;
        public string Cate_image
        {
            get => cate_image;
            set => SetProperty(ref cate_image, value);
        }


        [JsonPropertyName("create_date")]
        public string create_date;
        public string Create_date
        {
            get => create_date;
            set => SetProperty(ref create_date, value);
        }


        public DateTime create_datee;
        public DateTime Create_datee
        {
            get => create_datee;
            set
            {
                SetProperty(ref create_datee, value);
            }
        }


        [ObservableProperty]
        ImageSource imageSource;

        public IAsyncRelayCommand delItemCategory { get; }
        public IServiceProvider serviceProvider { get; set; }
        public ICategoryServices categoryServices { get; set; }
        public CategoryPageViewModel viewModel { get; set; }
        public AppShell appShell { get; set; }

        public Category(
            IServiceProvider serviceProvider,
            CategoryPageViewModel viewModel, 
            ICategoryServices categoryServices,
            AppShell appShell)
        {
            this.serviceProvider = serviceProvider;
            this.viewModel = viewModel;
            this.categoryServices = categoryServices;
            this.appShell = appShell;
            ImageSource = ImageSource.FromFile("dotnet_bot.png");
            delItemCategory = new AsyncRelayCommand<int>(RemoveItem);
        }

        public void SetDateTime()
        {
            if (!string.IsNullOrEmpty(Create_date))
            {
                Create_datee = DateTime.Parse(Create_date);
            }
        }

        private async Task UpdateUIAfterDelete(bool result)
        {
            if (result)
            {
                string title = "Del!";
                string message = "You'r deleted the item!";
                await Shell.Current.DisplayAlert(title, message, "OK!");
                await viewModel.LoadCategorys(true);
            }
            else
            {
                string title = "INFO";
                string message = "Can't delete the item, check the networking, please.";
                await Shell.Current.DisplayAlert(title, message, "OK!");
            }
        }

        public async Task RemoveItem(int id)
        {
            try
            {
                await ShowBusyIndicator();
                var result = await categoryServices.Del(id);
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                }

                await UpdateUIAfterDelete(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }
        }

        public async Task DownloadImageAsync()
        {
            // Get the image URL from your Cate_image property
            try
            {
                if(IsBusy || isReloadImage) return;

                string url = string.Format(FORMATS.DOWNLOAD_IMAGE, Cate_image);
                // Create an ImageSource from the URL
                Uri imageUrl = new Uri(url);
                using (HttpClient httpClient = new())
                {
                    // Download the image asynchronously
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                    // Create an ImageSource from the downloaded bytes
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        Stream stream = new MemoryStream(imageBytes);
                        ImageSource = ImageSource.FromStream(() => stream);
                    }
                    else
                    {
                        ImageSource = ImageSource.FromFile("dotnet_bot.png");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the download
                Console.WriteLine("Error downloading image: " + ex.Message);
                ImageSource = ImageSource.FromFile("dotnet_bot.png");
            }
            finally
            {
                IsBusy = false;
                isReloadImage = true;
            }
        }


        // Popup Busy indicator
        private BusyIndicator busyIndicator { get; set; }
        private async Task<bool> ShowBusyIndicator()
        {

            try
            {
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                    busyIndicator = null;
                }

                busyIndicator = new BusyIndicator();
                App.Current.MainPage.ShowPopup(busyIndicator);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }

            return await Task.FromResult(true);
        }
    }
}
