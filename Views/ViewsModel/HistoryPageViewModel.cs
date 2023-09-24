using CashY.Model.Items.Logger;
using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CashY.Views.ViewsModel
{
    public partial class HistoryPageViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<MyLogger> logger;

        [ObservableProperty]
        private bool isLoading;

        private readonly IDatabaseServices _databaseServices;
        public HistoryPageViewModel(IDatabaseServices databaseServices)
        {
            this._databaseServices = databaseServices;
            logger = new ObservableCollection<MyLogger>();

        }

        public async Task Reload()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                // clear array items.
                Logger.Clear();

                // check if the result not nullable or empty.
                if (_databaseServices.loggers != null || _databaseServices.loggers.Count == 0)
                {
                    foreach (var logger in _databaseServices.loggers)
                    {
                        Logger.Add(logger);
                    }

                    IsLoading = true;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Logger View Model Error:{ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
