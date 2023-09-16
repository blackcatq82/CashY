using CashY.ViewModels;
using CashY.Views;
namespace CashY;

public partial class AppShell : Shell
{
    private IServiceProvider provider { get; set; }
    public AppShell(ShellViewModel vm, IServiceProvider provider)
    {
        /// Injection Services
        BindingContext = vm;
        this.provider = provider;

        /// init
        InitializeComponent();


        /// register a pages item's
        //RegisterPages();
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

    #region Register Page's
    /// <summary>
    ///  Init register a page's
    /// </summary>
    //private void RegisterPages()
    //{
    //    /// using the scope services injection.
    //    using (var scope = provider.CreateScope())
    //    {
    //        // register home Page
    //        //RegisterPage<HomePage>(HomePage);
    //        //RegisterPage<CategoryPage>(CategoryPage);
    //        //RegisterPage<ItemsPage>(ItemsPage);
    //        //RegisterPage<PaymentPage>(PaymentPage);
    //        //RegisterPage<HistoryPage>(HistoryPage);
    //    }
    //}

    /// <summary>
    /// Register page in Content Shell.
    /// </summary>
    /// <typeparam name="T">type page in injection</typeparam>
    /// <param name="shell">property item ShellContent</param>
    //public void RegisterPage<T>(ShellContent shell) where T : class
    //{
    //    using (var scope = provider.CreateScope())
    //    {
    //        shell.Content = scope.ServiceProvider.GetRequiredService(typeof(T));
    //    }
    //}
    #endregion
}
