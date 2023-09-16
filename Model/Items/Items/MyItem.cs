using CashY.Core;
using CashY.Pop;
using CashY.Services;
using CashY.Views.ViewsModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;
namespace CashY.Model.Items.Items
{
    public partial class MyItem : ObservableObject, IDisposable
    {
        private int id;
        [JsonPropertyName("id")]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private int item_cate;
        [JsonPropertyName("item_cate")]
        public int Item_cate
        {
            get => item_cate;
            set => SetProperty(ref item_cate, value);
        }

        private string item_name;
        [JsonPropertyName("item_name")]
        public string Item_name
        {
            get => item_name;
            set => SetProperty(ref item_name, value);
        }


        private double item_price;
        [JsonPropertyName("item_price")]
        public double Item_price
        {
            get => item_price;
            set => SetProperty(ref item_price, value);
        }



        private string item_image;
        [JsonPropertyName("item_image")]
        public string Item_image
        {
            get => item_image;
            set => SetProperty(ref item_image, value);
        }

        private int item_quantity;
        [JsonPropertyName("item_quantity")]
        public int Item_quantity
        {
            get => item_quantity;
            set => SetProperty(ref item_quantity, value);
        }

        private string item_expire_date;
        [JsonPropertyName("item_expire_date")]
        public string Item_expire_date
        {
            get => item_expire_date;
            set => SetProperty(ref item_expire_date, value);
        }

        private string item_create_date;
        [JsonPropertyName("item_create_date")]
        public string Item_create_date
        {
            get => item_create_date;
            set => SetProperty(ref item_create_date, value);
        }



        private double item_price_buy;
        [JsonPropertyName("item_price_buy")]
        public double Item_price_buy
        {
            get => item_price_buy;
            set => SetProperty(ref item_price_buy, value);
        }

        [ObservableProperty]
        private DateTime create_date;

        [ObservableProperty]
        private DateTime expire_date;

        [ObservableProperty]
        private ImageSource imageSource;

        [ObservableProperty]
        private bool isReloadImage;
        public IAsyncRelayCommand Del { get; set; }

        public IItemsServices itemsServices { get; set; }

        public ItemsPageViewModel viewModel { get; set; }
        public MyItem(IItemsServices itemsServices, ItemsPageViewModel viewModel)
        {
            this.itemsServices = itemsServices;
            this.viewModel = viewModel;

            Del = new AsyncRelayCommand<int>(DelTask);
        }

        private async Task DelTask(int id)
        {
            try
            {
                await ShowBusyIndicator();
                var result = await itemsServices.Del(id);
                await UpdateUIAfterDelete(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                }
            }
        }


        private async Task UpdateUIAfterDelete(bool result)
        {
            if (result)
            {
                await viewModel.ReloadItems(true);
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                }

                string title = "Del!";
                string message = "You'r deleted the item!";

                await Shell.Current.DisplayAlert(title, message, "OK!");
            }
            else
            {
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                }

                string title = "INFO";
                string message = "Can't delete the item, check the networking, please.";
                await Shell.Current.DisplayAlert(title, message, "OK!");
            }
        }

        public Task DownloadImageAsync()
        {
            cancellationTokenSource = new CancellationTokenSource(); 
            token = cancellationTokenSource.Token;

            //await Task.Run(async () =>
            //{
            //    // Check for cancellation before starting the task
            //    cancellationTokenSource.Token.ThrowIfCancellationRequested();
            //    await DownloadImg();
            //}, token);

            ImageSource = ImageSource.FromFile("dotnet_bot.png");

            return Task.CompletedTask;
        }

        private async Task DownloadImg()
        {
            // Get the image URL from your Cate_image property
            try
            {
                if (IsReloadImage) return;
                IsReloadImage = true;

                string url = string.Format(FORMATS.DOWNLOAD_IMAGE_ITEMS, Item_image);
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
            catch (OperationCanceledException ex)
            {
                // Handle cancellation gracefully
                Console.WriteLine("Task was canceled: " + ex.Message);
                IsReloadImage = false;
                return;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the download
                Console.WriteLine("Error downloading image: " + ex.Message);
                ImageSource = ImageSource.FromFile("dotnet_bot.png");
                IsReloadImage = false;
                return;
            }
            finally
            {
                IsReloadImage = true;
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
                Shell.Current.ShowPopup(busyIndicator);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }

            return await Task.FromResult(true);
        }


        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken token;
        private bool disposing = true;
        public void Dispose()
        {
            if (cancellationTokenSource == null) return;
            cancellationTokenSource?.Cancel();
            if (disposing)
            {
                // Cancel the task and dispose the CancellationTokenSource
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                disposing = false;
                IsReloadImage = false;
            }
        }
    }
}
