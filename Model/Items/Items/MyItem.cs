using CashY.Services;
using CashY.Views.ViewsModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;
namespace CashY.Model.Items.Items
{
    public partial class MyItem : ObservableObject
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

        [ObservableProperty]
        private string item_cate_name;

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

        public readonly IItemsServices itemsServices;
        public readonly IPopupServices popupServices;
        private readonly ILoadData load;
        private readonly IDatabaseServices databaseServices;
        public readonly ItemsPageViewModel viewModel;
        public MyItem(IItemsServices itemsServices, IPopupServices popupServices, ILoadData load,  ItemsPageViewModel viewModel, IDatabaseServices databaseServices)
        {
            this.itemsServices = itemsServices;
            this.viewModel = viewModel;
            this.load = load;
            this.popupServices= popupServices;
            this.databaseServices=databaseServices;

            Del = new AsyncRelayCommand<int>(DelTask);

        }

        private async Task DelTask(int id)
        {
            try
            {
                popupServices.Show();
                var result = await itemsServices.Del(id);
                popupServices.Close();
                if (result)
                {
                    await popupServices.ShowPopUpMessage("Del!", "You'r deleted the item!", "OK!");
                    var item = viewModel.Items.Where(x => x.Id == id).FirstOrDefault();
                    if (item!= null)
                    {
                        viewModel.Items.Remove(item);
                    }
                }
                else
                {
                    await popupServices.ShowPopUpMessage("INFO", "Can't delete the item, check the networking, please.", "OK!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task reloadImage()
        {
            try
            {
                var cate = databaseServices.categories.Where(x=>x.Id == item_cate).FirstOrDefault();
                if (cate != null)
                {
                    Item_cate_name = cate.cate_name;
                }
                else
                {
                    Item_cate_name = "لا يوجد";
                }

                string pathImage = await this.load.GetPathItems(Id, Item_image);
                ImageSource = ImageSource.FromFile(pathImage);
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"Load Image Error:{ex}");
            }
        }


        public Task CopyTo(MyItemRequest request)
        {
            Id = request.Id;
            Item_name = request.item_name;
            Item_cate = request.item_cate;
            Item_image = request.item_image;
            Item_price = request.item_price;
            Item_price_buy = request.item_price_buy;
            Item_quantity = request.item_quantity;
            Item_create_date = request.item_create_date;
            Item_expire_date = request.item_expire_date;
            return Task.CompletedTask;
        }
    }
}
