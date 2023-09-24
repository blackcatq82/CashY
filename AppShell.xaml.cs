using CashY.Views;
namespace CashY;

public partial class AppShell : Shell
{
    public readonly HomePage homePage;
    public readonly CategoryPage categoryPage;
    public readonly ItemsPage itemsPage;
    public readonly PaymentPage paymentPage;
    public readonly HistoryPage historyPage;
    private readonly IServiceProvider provider;
    public AppShell(IServiceProvider provider, HomePage homePage, CategoryPage categoryPage, ItemsPage itemsPage, PaymentPage paymentPage, HistoryPage historyPage)
    {
        /// init
        InitializeComponent();

        this.provider = provider;


        // pages
        this.homePage = homePage;
        this.categoryPage = categoryPage;
        this.itemsPage = itemsPage;
        this.paymentPage = paymentPage;
        this.historyPage = historyPage;

        // injection pages
        this.HomePage.Content = this.homePage;
        this.CategoryPage.Content = this.categoryPage;
        this.ItemsPage.Content = this.itemsPage;
        this.PaymentPage.Content = this.paymentPage;
        this.HistoryPage.Content = this.historyPage;
    }

    /// <summary>
    /// Logout from account.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Logout_Clicked(object sender, EventArgs e)
    {
        // Push the new page onto the navigation stack
        App.Current.MainPage = provider.GetRequiredService<Login>();
    }
}
