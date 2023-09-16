using CashY.Model.Items.Items;
using CashY.Pop;
using CashY.Pop.Items;
using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.ObjectModel;

namespace CashY.Views.ViewsModel
{
    public partial class  ItemsPageViewModel : NewBaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<MyItem> items;

        [ObservableProperty]
        public ObservableCollection<MyItem> filteredItems;

        private MyItem[] array;

        public IAsyncRelayCommand addNewItem { get; set; }
        public IAsyncRelayCommand search { get; set; }

        private IServiceProvider serviceProvider { get; set; }

        private IItemsServices services { get; set; }

        [ObservableProperty]
        private bool isSearching;

        private int RowPerPage = 15;
        [ObservableProperty]
        private int rowsCount;

        [ObservableProperty]
        private int currentPage = 1;


        [ObservableProperty]
        private bool hasNextPage;
        [ObservableProperty]
        private bool hasPreviousPage;

        public IAsyncRelayCommand nextPage { get; set; }
        public IAsyncRelayCommand previousPage { get; set; }

        // Popup Busy indicator
        private BusyIndicator busyIndicator { get; set; }

        public ItemsPageViewModel(IServiceProvider serviceProvider, IItemsServices services)
        {
            this.serviceProvider = serviceProvider;
            this.services = services;

            items = new ObservableCollection<MyItem>();
            filteredItems = new ObservableCollection<MyItem>();


            addNewItem = new AsyncRelayCommand(TaskAddNewItem);
            search = new AsyncRelayCommand<string>(TaskSearch);
            nextPage = new AsyncRelayCommand(NextPage);
            previousPage = new AsyncRelayCommand(PreviousPage);

            RowPerPage = 15;
            CurrentPage = 1;
            hasNextPage = true;
            hasPreviousPage = false;

        }

        public async Task ReloadItems(bool hasPopScreen = false)
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
                // get result from respone api get Items.
                var result = await services.GetItemsAsync();

                // clear array items.
                Items.Clear();
                array = new MyItem[result.Length];
                RowsCount = result.Length;

                int index = 0;
                // check if the result not nullable or empty.
                if (result != null || result.Length == 0)
                {
                    foreach (var item in result)
                    {
                        if (item.ImageSource == null)
                            item.ImageSource = ImageSource.FromFile("dotnet_bot.png");

                        await item.DownloadImageAsync();

                        if(Items.Count < RowPerPage)
                            Items.Add(item);

                        array[index] = item;
                        index++;
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
        public async Task NextPage()
        {
            try
            {
                if(IsBusy) return;
                // Start to show Busy Indicator.
                await ShowBusyIndicator();
                if (HasNextPage)
                {
                    CurrentPage++;
                    HasNextPage = false;
                    // Load the data for the next page.
                    // Calculate the start index for the current page
                    int startIndex = (CurrentPage - 1) * RowPerPage;

                    // Calculate the end index for the current page
                    int endIndex = startIndex + RowPerPage;

                    // Start selected rows
                    if (startIndex >= 0 && endIndex <= array.Length)
                    {
                        await Items.ClearDispose();
                        var currentItems = new List<MyItem>(array.Skip(startIndex).Take(RowPerPage).ToList());
                        foreach (var item in currentItems)
                        {
                            await item.DownloadImageAsync();
                            Items.Add(item);
                        }
                    }
                    UpdatePageNavigation();
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            finally
            {
                if(busyIndicator is not null)
                    busyIndicator.Close();

                IsBusy = false;
            }
        }


        public async Task PreviousPage()
        {
            try
            {
                if (IsBusy) return;
                // Start to show Busy Indicator.
                await ShowBusyIndicator();
                if (HasPreviousPage)
                {
                    HasPreviousPage = false;
                    CurrentPage--;
                    // Calculate the start index for the current page
                    int startIndex = (CurrentPage - 1) * RowPerPage;

                    // Calculate the end index for the current page
                    int endIndex = startIndex + RowPerPage;

                    // Start selected rows
                    if (startIndex >= 0 && endIndex <= array.Length)
                    {
                        await Items.ClearDispose();
                        Items.Clear();
                        var currentItems = new List<MyItem>(array.Skip(startIndex).Take(RowPerPage).ToList());
                        foreach (var item in currentItems)
                        {
                            await item.DownloadImageAsync();
                            Items.Add(item);
                        }
                    }

                    UpdatePageNavigation();
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            finally
            {
                if (busyIndicator is not null)
                    busyIndicator.Close();

                IsBusy = false;
            }
        }

        public void UpdatePageNavigation()
        {
            HasNextPage = CurrentPage * RowPerPage < RowsCount;
            HasPreviousPage = CurrentPage > 1;
        }

        private async Task TaskSearch(string search)
        {
            if(IsBusy) return;
            try
            {
                IsBusy = true;
                if (string.IsNullOrEmpty(search)) return;

                await ShowBusyIndicator();

                string searchText = search.ToLower();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    IsSearching = true;

                    var filteredList = array.Where(item => item.Item_name.ToLower().Contains(searchText)).ToList();
                    FilteredItems.Clear();
                    foreach (var item in filteredList)
                    {
                        FilteredItems.Add(item);
                    }

                }
                else
                {
                    IsSearching = false;
                    FilteredItems.Clear();
                }


                if (busyIndicator is not null)
                {
                    await busyIndicator.CloseAsync();
                    busyIndicator = null;
                }
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error Search  :{ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task TaskAddNewItem()
        {
            using(var scope = serviceProvider.CreateScope())
            {
                var insertNewPage = scope.ServiceProvider.GetService<PopupInsertItem>();
                await Shell.Current.Navigation.PushAsync(insertNewPage);
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

    public static class ItemHelper
    {
        public static Task ClearDispose(this global::System.Collections.ObjectModel.ObservableCollection<global::CashY.Model.Items.Items.MyItem> Items)
        {
            foreach(var item in Items)
            {
                item.Dispose();
            }   
            
            return Task.CompletedTask;
        }
    }
}
