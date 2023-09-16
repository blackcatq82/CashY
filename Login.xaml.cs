using CashY.ViewModels;
namespace CashY;
public partial class Login : ContentPage
{
	public Login(LoginViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
    }
}


