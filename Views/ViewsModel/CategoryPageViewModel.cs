using CashY.ViewModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CashY.Pop.Category;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CashY.Model.Items;
using CashY.Services;
namespace CashY.Views.ViewsModel
{
    public partial class CategoryPageViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        private bool isRefreshing;
        [ObservableProperty]
        private bool isSearching = false;

        // Array category items
        [ObservableProperty]
        private ObservableCollection<Category> categorys;


        // Array category search
        [ObservableProperty]
        private ObservableCollection<Category> filteredCategories;

        [ObservableProperty]
        private bool isLoading;

        // Create
        public IAsyncRelayCommand addnewCategory { get; set; }

        // search
        public IAsyncRelayCommand search { get; set; }

        // Serivces
        private readonly IServiceProvider serviceProvider;
        private readonly ILoadData load;
        private readonly IPopupServices popupServices;
        private readonly IDatabaseServices databaseServices;
        // ctor
        public CategoryPageViewModel(IDatabaseServices databaseServices, IPopupServices popupServices, IServiceProvider serviceProvider, ILoadData load)
        {
            // Set services
            this.serviceProvider  = serviceProvider;
            this.popupServices    = popupServices;
            this.load             = load;
            this.databaseServices = databaseServices;

            // Set array items
            categorys          = new ObservableCollection<Category>();
            FilteredCategories = new ObservableCollection<Category>();

            // Handler Async Relay Commands
            addnewCategory     = new AsyncRelayCommand(OpenCreateNewCategory);

            // search
            search = new AsyncRelayCommand<string>(SearchByName);
        }

        private async Task OpenCreateNewCategory()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var cateInsert = scope.ServiceProvider.GetRequiredService<CateInsert>();
                await Shell.Current.ShowPopupAsync(cateInsert);
            }
        }


        /// <summary>
        /// Reload a category
        /// </summary>
        /// <returns></returns>
        public async Task LoadCategorysExecute()  => await LoadCategorys();
        public async Task LoadCategorys()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                // clear array items.
                Categorys.Clear();

                // check if the result not nullable or empty.
                if (databaseServices.categories != null || databaseServices.categories.Count == 0)
                {
                    foreach (var category in databaseServices.categories)
                    {
                        await category.ReloadImage();
                        if (category.ImageSource == null)
                            category.ImageSource = ImageSource.FromFile("dotnet_bot.png");

                        Categorys.Add(category);
                    }
                    IsLoading = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"On Load Category mvvm : {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }

            return;
        }
        private async Task SearchByName(string cate_name)
        {
            try
            {
                if (string.IsNullOrEmpty(cate_name)) return;
                var categorypage = serviceProvider.GetRequiredService<CategoryPage>();
                popupServices.Show(categorypage);

                string searchText = cate_name.ToLower();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    IsSearching = true;
                    if (IsBusy) return;
                    IsBusy = true;

                    if (databaseServices.categories == null || databaseServices.categories.Count == 0)
                    {
                        await GetDataSocket();
                    }

                    var filteredList = databaseServices.categories.Where(category => category.Cate_name.ToLower().Contains(searchText)).ToList();
                    foreach (var category in filteredList)
                    {
                        await category.ReloadImage();
                    }
                    Categorys = new ObservableCollection<Category>(filteredList);
                }
                else
                {
                    IsSearching = false;
                    await LoadCategorysExecute();
                }

            }
            finally
            {
                popupServices.Close();
            }
        }

        public async Task GetDataSocket()
        {
            await load.ReloadingData();
            await LoadCategorysExecute();
        }
    }
}

