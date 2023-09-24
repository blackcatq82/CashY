using CashY.Views;
namespace CashY;
public partial class App : Application
{
    public App(LoadPage load)
	{
        InitializeComponent();
        MainPage = load;
    }
}
