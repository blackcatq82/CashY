using CashY.Model.Items.Payment;
using CashY.Services;
using CashY.ViewModels;
using System.Collections.ObjectModel;
namespace CashY.Views.ViewsModel
{
    public partial class PaymentPageViewModel : NewBaseViewModel
    {
        private ObservableCollection<MyPayment> payments;
        public ObservableCollection<MyPayment> Payments
        {
            get => payments;
            set => SetProperty(ref payments, value);
        }

        private readonly IPaymentServices paymentServices;
        private readonly IPopupServices popupServices;
        private readonly IServiceProvider serviceProvider;
        public PaymentPageViewModel(IPaymentServices paymentServices, IPopupServices popupServices, IServiceProvider serviceProvider)
        {
            this.paymentServices = paymentServices;
            this.popupServices=popupServices;
            this.serviceProvider=serviceProvider;

            Payments = new ObservableCollection<MyPayment>();

        }

        public async Task Reload()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var PaymetPage = serviceProvider.GetRequiredService<PaymentPage>();
                popupServices.Show(PaymetPage);

                // get result from respone api get Items.
                var result = await paymentServices.GetPaymentsAsync();

                // clear array items.
                Payments.Clear();

                // check if the result not nullable or empty.
                if (result != null || result.Length == 0)
                {
                    foreach (var item in result)
                    {
                        await Shell.Current.Dispatcher.DispatchAsync(() =>
                        {

                            Payments.Add(item);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                IsBusy = false;
                popupServices.Close();;
            }
        }
    }
}
