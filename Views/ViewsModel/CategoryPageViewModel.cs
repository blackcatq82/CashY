using CashY.Model;
using CashY.ViewModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CashY.Pop.Category;
using CashY.Pop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CashY.Model.Items;
using CashY.Services;
namespace CashY.Views.ViewsModel
{
    public partial class CategoryPageViewModel : NewBaseViewModel
    {
        private Category[] baseArrayItems;

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

        // Reload
        public IAsyncRelayCommand reloadCategory { get; set; }

        // Create
        public IAsyncRelayCommand addnewCategory { get; set; }

        // search
        public IAsyncRelayCommand search { get; set; }

        // refresh
        public IAsyncRelayCommand refreshCommand { get; set; }

        // Serivces
        private IServiceProvider serviceProvider { get; set; }

        // category service
        private ICategoryServices categoryServices { get; set; }

        // Popup Busy indicator
        private BusyIndicator busyIndicator { get; set; }

        // ctor
        public CategoryPageViewModel(ICategoryServices categoryServices, IServiceProvider serviceProvider)
        {
            // Set services
            this.categoryServices = categoryServices;
            this.serviceProvider  = serviceProvider;

            // Set array items
            categorys          = new ObservableCollection<Category>();
            FilteredCategories = new ObservableCollection<Category>();

            // Handler Async Relay Commands
            reloadCategory     = new AsyncRelayCommand(LoadCategorysExecute);
            addnewCategory     = new AsyncRelayCommand(OpenCreateNewCategory);
            refreshCommand     = new AsyncRelayCommand(ExecuteRefreshCommand);

            // search
            search = new AsyncRelayCommand<string>(SearchByName);
        }

        private async Task ExecuteRefreshCommand()
        {
            // Set IsRefreshing to true to show the refresh indicator
            IsRefreshing = true;

            try
            {
                // Perform your refresh logic here
                await LoadCategorys();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the refresh

                // Optionally, you can display an error message or log the exception.
            }
            finally
            {
                // Set IsRefreshing to false to hide the refresh indicator
                IsRefreshing = false;
            }
        }
        private async Task SearchByName(string cate_name)
        {
            if (string.IsNullOrEmpty(cate_name)) return;

            await ShowBusyIndicator();

            string searchText = cate_name.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                IsSearching = true;

                // get result from respone api get categorys.
                var result = await categoryServices.GetCategoriesAsync();

                // check if the result not nullable or empty.
                if (result != null || result.Length == 0)
                    baseArrayItems = result;

                var filteredList = baseArrayItems.Where(category => category.Cate_name.ToLower().Contains(searchText)).ToList();
                FilteredCategories = new ObservableCollection<Category>(filteredList);
            }
            else
            {
                IsSearching = false;
                FilteredCategories = new ObservableCollection<Category>(Categorys);
            }


            if (busyIndicator is not null)
            {
                await busyIndicator.CloseAsync();
                busyIndicator = null;
            }
        }
        // Insert function
        private async Task OpenCreateNewCategory()
        {
            using(var scope = serviceProvider.CreateScope())
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
        public async Task LoadCategorys(bool hasPopScreen = false)
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                if (!hasPopScreen)
                {
                    // Start to show Busy Indicator.
                    await ShowBusyIndicator();
                }
                // get result from respone api get categorys.
                var result = await categoryServices.GetCategoriesAsync();

                // clear array items.
                Categorys.Clear();

                // check if the result not nullable or empty.
                if (result != null || result.Length == 0)
                {
                    foreach (var category in result)
                    {
                        if(category.ImageSource == null)
                            category.ImageSource = ImageSource.FromFile("dotnet_bot.png");

                        Categorys.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                IsBusy = false;

                // busy indicator close
                if (!hasPopScreen)
                {
                    await busyIndicator.CloseAsync();
                    busyIndicator = null;
                }
            }
        }

        /// <summary>
        /// Show Busy Indicator Popup!
        /// </summary>
        private Task ShowBusyIndicator()
        {
            if (busyIndicator is not null)
                return Task.CompletedTask;

            using (var Scope = serviceProvider.CreateScope())
            {
                busyIndicator = Scope.ServiceProvider.GetService<BusyIndicator>();
                if (busyIndicator is not null)
                {
                    Shell.Current.ShowPopup(busyIndicator);
                }
            }

            return Task.CompletedTask;
        }
    }
}

