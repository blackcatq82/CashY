using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;

namespace CashY.Views.ViewsModel
{
    public partial class HomePageViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        private int countCategorys;
        [ObservableProperty]
        private int countItems;
        [ObservableProperty]
        private int countHistory;
        [ObservableProperty]
        private int countPayment;

        [ObservableProperty]
        private double totalPrice;

        private readonly IDatabaseServices databaseServices;
        public HomePageViewModel(IDatabaseServices databaseServices)
        {
            this.databaseServices = databaseServices;
        }


        public async Task Reload()
        {
            try
            {
                CountCategorys = databaseServices.categories.Count();
                CountItems     = databaseServices.items.Count();
                CountPayment   = databaseServices.payments.Count();
                CountHistory   = databaseServices.loggers.Count();


                using(var connection =  databaseServices.GetConnection())
                {
                    string query = "SELECT SUM(price) AS total_price FROM payments WHERE date_create >= DATE_SUB(NOW(), INTERVAL 30 DAY);";
                    TotalPrice = await connection.ExecuteScalarAsync<double>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
