using CashY.Model.Items.Items;
using CashY.Pop.Items;
using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashY.Views.ViewsModel
{
    public partial class  ItemsPageViewModel : NewBaseViewModel
    {

        private ObservableCollection<MyItem> items;
        public ObservableCollection<MyItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }



        private MyItem[] array;

        [ObservableProperty]
        private int currentPage;

        public IAsyncRelayCommand GetNext { get; set; }
        public IAsyncRelayCommand GetBack { get; set; }

        public IAsyncRelayCommand addNewItem { get; set; }
        public IAsyncRelayCommand search { get; set; }


        [ObservableProperty]
        private bool isSearching;

        [ObservableProperty]
        private bool isLoading;

        private readonly IServiceProvider serviceProvider;
        private readonly IItemsServices services;
        private readonly IPopupServices popupServices;
        private readonly ILoadData load;
        public ItemsPageViewModel(IServiceProvider serviceProvider, IItemsServices services, IPopupServices popupServices, ILoadData load)
        {
            this.serviceProvider = serviceProvider;
            this.services = services;
            this.popupServices = popupServices;
            this.load=load;
            items = new ObservableCollection<MyItem>();


            addNewItem = new AsyncRelayCommand(TaskAddNewItem);
            search = new AsyncRelayCommand<string>(TaskSearch);

        }
        public async Task GetDataSocket()
        {
            await load.ReloadingData();
            array = await services.GetItemsAsync();
            await ReloadItems();
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
                    var itemsPage = serviceProvider.GetRequiredService<ItemsPage>();
                    popupServices.Show(itemsPage);
                }
                if(array == null || array.Length == 0)
                {
                    // get result from respone api get Items.
                    array = await services.GetItemsAsync();
                }


                // clear array items.
                Items.Clear();

                // check if the result not nullable or empty.
                if (array != null || array.Length == 0)
                {
                    foreach (var item in array)
                    {
                        await item.reloadImage();

                        if (item.ImageSource == null)
                            item.ImageSource = ImageSource.FromFile("dotnet_bot.png");

                        Items.Add(item);
                    }

                    IsLoading = true;
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
                    popupServices.Close();;
                }
            }
        }
        [RelayCommand]
        public async Task GetNextAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;


                // Start to show Busy Indicator.
                var itemsPage = serviceProvider.GetRequiredService<ItemsPage>();
                CurrentPage = services.currentCount;
                popupServices.Show(itemsPage);


                Console.WriteLine("Start");
                var result = await services.GetNext();
                if(result != null && result.Length > 0)
                {
                    foreach (var item in result)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("Outting");
                    await Console.Out.WriteLineAsync("No more items!");
                    await popupServices.ShowPopUpMessage("Info", "No more items!", "OK!");
                }
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error in reload items more :{ex}");
            }
            finally
            {
                IsBusy = false;
                popupServices.Close();
            }
        }
        [RelayCommand]
        public async Task GetBackAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;


                // Start to show Busy Indicator.
                var itemsPage = serviceProvider.GetRequiredService<ItemsPage>();
                popupServices.Show(itemsPage);



                var result = await services.GetBack();
                if (result != null && result.Length > 0)
                {
                    Items.Clear();
                    foreach (var item in result)
                    {
                        await item.reloadImage();

                        if (item.ImageSource == null)
                            item.ImageSource = ImageSource.FromFile("dotnet_bot.png");

                        Items.Add(item);
                    }
                }
                else
                {
                    await Console.Out.WriteLineAsync("No more items!");
                    await popupServices.ShowPopUpMessage("Info", "No more items!", "OK!");
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error in reload items more :{ex}");
            }
            finally
            {
                IsBusy = false;
                popupServices.Close();
            }
        }
        private async Task TaskSearch(string search)
        {
            if(IsBusy) return;
            try
            {
                IsBusy = true;
                if (string.IsNullOrEmpty(search)) return;

                var itemsPage = serviceProvider.GetRequiredService<ItemsPage>();
                popupServices.Show(itemsPage);

                string searchText = search.ToLower();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    IsSearching = true;
                    if(array == null || array.Length == 0)
                    {
                        array = await services.GetItemsAsync();
                    }

                    var filteredList = array.Where(item => item.Item_name.ToLower().Contains(searchText)).ToList();

                    Items.Clear();

                    foreach (var item in filteredList)
                    {
                        Items.Add(item);
                    }

                }
                else
                {
                    IsSearching = false;
                    await ReloadItems();
                }
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error Search  :{ex}");
            }
            finally
            {
                IsBusy = false;
                popupServices.Close();;
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
    }
}
