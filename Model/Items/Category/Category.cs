using CashY.Services;
using CashY.ViewModels;
using CashY.Views.ViewsModel;
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



        private readonly ICategoryServices categoryServices;
        private readonly IPopupServices popupServices;
        private readonly ILoadData load;
        private readonly CategoryPageViewModel viewModel;

        public Category(CategoryPageViewModel viewModel, ICategoryServices categoryServices, IPopupServices popupServices, ILoadData load)
        {
            this.viewModel = viewModel;
            this.categoryServices = categoryServices;
            this.popupServices=popupServices;
            this.load=load;

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

        public async Task RemoveItem(int id)
        {
            try
            {
                popupServices.Show();
                var result = await categoryServices.Del(id);
                if (result)
                {
                    popupServices.Close();
                    await popupServices.ShowPopUpMessage("Del!", "You'r deleted the item!", "OK!");
                    var item =  viewModel.Categorys.Where(x => x.Id == id).FirstOrDefault();
                    if (item!= null)
                    {
                        viewModel.Categorys.Remove(item);
                    }
                }
                else
                {
                    popupServices.Close();
                    await popupServices.ShowPopUpMessage("INFO", "Can't delete the item, check the networking, please.", "OK!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task ReloadImage()
        {
            // Get the image URL from your Cate_image property
            try
            {
                if(IsBusy || isReloadImage) return;
                string path = await load.GetPathCates(Id, Cate_image);
                ImageSource = ImageSource.FromFile(path);
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
        public Task CopyTo(CategoryRequest request)
        {
            this.Id = request.id;
            this.Cate_name = request.cate_name;
            this.Cate_image = request.cate_image;
            this.Create_datee = request.create_date;
            this.Create_date = request.create_date.ToShortDateString();
            return Task.CompletedTask;
        }
    }
}
