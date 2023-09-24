using CashY.Services;
using CashY.Views.ViewsModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;

namespace CashY.Model.Items.Payment
{
    public partial class MyPayment : ObservableObject
    {
        private int id;
        [JsonPropertyName("id")]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private double price;
        [JsonPropertyName("price")]
        public double Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        private string date_create;
        [JsonPropertyName("date_create")]
        public string Date_create
        {
            get => date_create;
            set => SetProperty(ref date_create, value);
        }

        private string items_ids;
        [JsonPropertyName("items_ids")]
        public string Items_ids
        {
            get => items_ids;
            set => SetProperty(ref items_ids, value);
        }

        private string items_names;
        [JsonPropertyName("date_create")]
        public string Items_names
        {
            get => items_names;
            set => SetProperty(ref items_names, value);
        }

        private string items_count;
        [JsonPropertyName("date_create")]
        public string Items_count
        {
            get => items_count;
            set => SetProperty(ref items_count, value);
        }

        [ObservableProperty]
        private DateTime create_date;


        public IAsyncRelayCommand delItemCategory { get; }


        public readonly IPaymentServices paymentServices;
        public readonly IServiceProvider serviceProvider;
        private readonly IPopupServices popupServices;
        private readonly PaymentPageViewModel paymentPageViewModel;
        public MyPayment(IServiceProvider serviceProvider, IPaymentServices paymentServices, IPopupServices popupServices, PaymentPageViewModel paymentPageViewModel)
        {
            this.paymentServices = paymentServices;
            this.serviceProvider = serviceProvider;
            this.paymentPageViewModel = paymentPageViewModel;
            this.popupServices = popupServices;

            delItemCategory = new AsyncRelayCommand(RemoveItem);
        }

        public async Task RemoveItem()
        {
            try
            {
                var result_del = await Shell.Current.DisplayAlert("Did you want to del?", $"Are you want to delete order #{this.Id}", "Yes" , "No");
                if (!result_del)
                    return;

                popupServices.Show();
                var result = await paymentServices.Del(this.Id);
                popupServices.Close();
                if (result)
                {
                    await popupServices.ShowPopUpMessage("Del!", "You'r deleted the item!", "OK!");
                    var item = paymentPageViewModel.Payments.Where(x=> x.Id ==  this.Id).FirstOrDefault();
                    if(item != null)
                        paymentPageViewModel.Payments.Remove(item);
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

        public Task CopyTo(PaymentRequest request)
        {
            this.Id = request.id;
            this.Items_count = request.items_count;
            this.Items_ids = request.items_ids;
            this.Items_names = request.items_names;
            this.Create_date = request.date_create;
            this.Price = request.price;
            return Task.CompletedTask;
        }
    }
}
