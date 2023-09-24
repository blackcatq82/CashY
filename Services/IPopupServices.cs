using CashY.Pop;
using CommunityToolkit.Maui.Views;

namespace CashY.Services
{
    public interface IPopupServices
    {
        BusyIndicator busyIndicator { get; set; }
        Task ShowPopUpMessage(string title, string messageBody, string buttonText);
        void Show(Page currentPage = null);
        Task Close();
    }

    public class PopupServices : IPopupServices
    {
        private BusyIndicator _busyIndicator;
        public BusyIndicator busyIndicator { get => _busyIndicator; set => _busyIndicator = value; }

        private readonly IServiceProvider serviceProvider;

        public PopupServices(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task ShowPopUpMessage(string title, string messageBody, string buttonText)
        {
            await App.Current.MainPage.DisplayAlert(title, messageBody, buttonText);
        }

        public void Show(Page currentPage = null)
        {
            if(currentPage == null)
            {
                currentPage = Shell.Current;
            }
            if(this.busyIndicator != null)
                _ = this.busyIndicator.CloseAsync();
            using (var scope = this.serviceProvider.CreateScope())
            {
                this.busyIndicator = scope.ServiceProvider.GetRequiredService<BusyIndicator>();
                if (this.busyIndicator != null)
                    currentPage.ShowPopup(this.busyIndicator);
            }
        }

        public async Task Close()
        {
            if(this.busyIndicator != null)
            {
                await this.busyIndicator.CloseAsync();
            }
        }
    }
}
