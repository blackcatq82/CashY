using CashY.Pop;
using CashY.Services;
using CommunityToolkit.Maui.Views;
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
        private ILoginService loginService { get; set; }
        private IServiceProvider  ServiceProvider { get; set; }

        private AppShell appShell { get; set; }
        public LoginViewModel(ILoginService loginService, IServiceProvider serviceProvider, AppShell appShell)
        {
            this.loginService = loginService;
            this.ServiceProvider=serviceProvider;
            this.appShell = appShell;


            username = "blackcat";
            password = "Xxionfeng22";
            loginclick = new AsyncRelayCommand(async () =>
            {
                if (IsBusy) return;
                IsBusy = true;
                await ShowBusyIndicator();
                var result = await this.loginService.LoginRequestHttp(username, password);

                if (busyIndicator != null)
                    busyIndicator.Close();

                if (result.Item1)
                {
                    App.Current.MainPage = appShell;
                }
                else
                {
                    await ShowPopUpMessage(result.Item2, "OK");
                }

                IsBusy = false;
            });

        }


        private BusyIndicator busyIndicator { get; set; }
        private async Task<bool> ShowBusyIndicator()
        {

            try
            {
                if (busyIndicator != null)
                {
                    // busy indicator close
                    busyIndicator.Close();
                    busyIndicator = null;
                }

                using (var scope = ServiceProvider.CreateScope())
                {
                    busyIndicator = scope.ServiceProvider.GetRequiredService<BusyIndicator>();
                    var shellApp = scope.ServiceProvider.GetRequiredService<Login>();

                    if (busyIndicator != null && shellApp != null)
                    {
                        await shellApp.Dispatcher.DispatchAsync(new Action(() =>
                        {
                            shellApp.ShowPopup(busyIndicator);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error:{ex.Message}");
            }

            return await Task.FromResult(true);
        }
        /// <summary>
        /// Popup Message view.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cLOSE"></param>
        /// <returns></returns>
        private async Task ShowPopUpMessage(string messageBody, string buttonText)
        {
            await App.Current.MainPage.DisplayAlert("Info", messageBody, buttonText);
        }
    }
}
