using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CashY.Views.ViewsModel
{
    public partial class LoadPageViewModel : NewBaseViewModel
    {
        private readonly IDatabaseServices databaseServices;
        private readonly ILoadData load;
        [ObservableProperty]
        private string filename;
        [ObservableProperty]
        private string message;
        [ObservableProperty]
        private int countfiles;
        [ObservableProperty]
        private int currentIndex;

        [ObservableProperty]
        private bool isComplated;

        [ObservableProperty]
        private long datalength;
        [ObservableProperty]
        private long currentBytesLength;
        [ObservableProperty]
        private int totatfiles;

        [ObservableProperty]
        private double myProgress;

        private readonly CategoryPageViewModel categoryPageViewModel;
        private readonly ItemsPageViewModel itemsPageViewModel;

        public LoadPageViewModel(ILoadData load, IDatabaseServices databaseServices, CategoryPageViewModel categoryPageViewModel, ItemsPageViewModel itemsPageViewModel)
        {
            this.load = load;
            this.databaseServices=databaseServices;
            this.categoryPageViewModel=categoryPageViewModel;
            this.itemsPageViewModel=itemsPageViewModel;

        }

        public async Task reload()
        {
            await databaseServices.Startup();
            await load.ReloadingData();
            IsComplated = true;
        }

        public Task UpdateUI()
        {
            Filename = load.filename;
            Message = load.message;
            Countfiles = load.countfiles;
            CurrentIndex = load.currentIndex;
            Datalength = load.datalength;
            CurrentBytesLength = load.currentBytesLength;
            Totatfiles = load.totatfiles;

            MyProgress = (double)CurrentIndex / Totatfiles;

            return Task.CompletedTask;
        }
    }
}
