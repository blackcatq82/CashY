using CashY.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace CashY.ViewModels
{
    public partial class LoginViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string password;

        public IAsyncRelayCommand loginclick { get; set; }

        public readonly ILoginService loginService;
        public readonly IIPermissionServices _PermissionServices;
        public readonly IPopupServices popupServices;
        public readonly AppShell appShell;

        public LoginViewModel(ILoginService loginService, IIPermissionServices _PermissionServices, IPopupServices popupServices, AppShell appShell)
        {
            this.loginService = loginService;
            this._PermissionServices = _PermissionServices;
            this.popupServices = popupServices;

            this.appShell = appShell;

            Username = "blackcat";
            Password = "Xxionfeng22";
            loginclick = new AsyncRelayCommand(TaskLoginRequest);

        }

        private async Task TaskLoginRequest()
        {
            if (IsBusy) return;
            IsBusy = true;

            popupServices.Show(App.Current.MainPage);
            var result = await loginService.LoginRequestHttp(Username, Password);
            popupServices.Close();


            if (result.Item1)
            {
                App.Current.MainPage = appShell;
            }
            else
            {
                await popupServices.ShowPopUpMessage("INFO", result.Item2, "OK");
            }


            IsBusy = false;
        }

        public async Task Reload()
        {
            _ = await _PermissionServices.GetRequestPermissionNetworking();
            _ = await _PermissionServices.GetRequestPermissionPhotos();
            _ = await _PermissionServices.GetRequestPermissionStorageWrite();
            _ = await _PermissionServices.GetRequestPermissionStorageRead();
            
        }
    }
}
