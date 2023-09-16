using CashY.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CashY.ViewModels
{
    public partial class ShellViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        public HomePage homePage;

        [ObservableProperty]
        public CategoryPage categoryPage;

        [ObservableProperty]
        public ItemsPage itemsPage;


        [ObservableProperty]
        public PaymentPage paymentPage;


        [ObservableProperty]
        public HistoryPage historyPage;

        public ShellViewModel(HomePage homePage, CategoryPage categoryPage,
                              ItemsPage itemsPage, PaymentPage paymentPage, HistoryPage historyPage)
        {
            this.homePage = homePage;
            this.categoryPage = categoryPage;
            this.itemsPage = itemsPage;
            this.paymentPage = paymentPage;
            this.historyPage = historyPage;

        }
    }
}
