namespace CashY;
public partial class App : Application
{
    public App(Login login)
	{
        InitializeComponent();
        MainPage = login;
    }
}
