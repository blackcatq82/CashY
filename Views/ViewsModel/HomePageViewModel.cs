using CashY.Model;
using CashY.Services;
using CashY.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CashY.Views.ViewsModel
{
    public partial class HomePageViewModel : NewBaseViewModel
    {
        AccountPackage accountPackage;
        public AccountPackage AccountPackage
        {
            get { return accountPackage; }
            set { SetProperty(ref accountPackage, value); }
        }

        [ObservableProperty]
        private int countCategorys;


        [ObservableProperty]
        private int countItems;


        private ICategoryServices categoryServices;
        private IItemsServices itemsServices;
        public HomePageViewModel(ICategoryServices categoryServices, IItemsServices itemsServices)
        {
            this.itemsServices=itemsServices;
            this.categoryServices=categoryServices;
            AccountPackage = AccountHelper.package;
        }


        public async Task Reload()
        {
            try
            {
                CountCategorys = 0;
                CountItems = 0;


                var result = await categoryServices.GetCategoriesAsync();
                CountCategorys = result.Count();

                var resultt = await itemsServices.GetItemsAsync();
                CountItems = resultt.Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
