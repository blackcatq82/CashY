using CashY.Core;
using CashY.Model.Items.Items;
using Newtonsoft.Json;

namespace CashY.Services
{
    public interface IItemsServices
    {
        Task<ItemPackage> InsertAsync(int item_cate, string item_name, double item_price, string item_image, int item_quantity, string item_expire_date, double item_price_buy);
        Task<MyItem[]> GetItemsAsync();

        Task<bool> Del(int id);
    }

    public class ItemsServices : IItemsServices
    {
        private string ClassName = "Items";
        private string MethodInsert = "Insert";
        private string MethodGetAll = "GetAll";
        private string MethodDel = "Del";

        private IServiceProvider _serviceProvider;
        private IMyHttpRequests _httpRequests;

        private ItemPackage package { get; set; }
        public ItemsServices(IServiceProvider serviceProvider, IMyHttpRequests httpRequests)
        {
            _serviceProvider = serviceProvider;
            _httpRequests = httpRequests;

        }

        #region Get All
        public async Task<MyItem[]> GetItemsAsync()
        {
            // Set format api call api class and method.
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodGetAll);
            MyItemRequest bodyData = new()
            {
                item_name = "GetAll",
                item_image = "GetAll",
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            _httpRequests.timeOut = 250;
            var result = await _httpRequests.PutAsync(url, jsonData);

            if (result.Item1)
            {
                if (result.Item2.Contains("getall"))
                {
                    if (result.Item2.Contains("status") || result.Item2.Contains("message") || result.Item2.Contains("error"))
                    {
                        package = JsonConvert.DeserializeObject<ItemPackage>(result.Item2);

                        MyItem[] array = new MyItem[package.Items.Length];
                        int index = 0;
                        foreach (var item in package.Items)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var itemNew = scope.ServiceProvider.GetRequiredService<MyItem>();
                                itemNew.Id  = item.Id;
                                itemNew.Item_cate = item.Item_cate;
                                itemNew.Item_name = item.Item_name;
                                itemNew.Item_price = item.Item_price;
                                itemNew.Item_quantity = item.Item_quantity;
                                itemNew.Item_image = item.Item_image;
                                itemNew.Item_create_date = item.Item_create_date;
                                itemNew.Item_expire_date = item.Item_expire_date;
                                itemNew.Item_price_buy = item.Item_price_buy;


                                itemNew.Create_date = DateTime.TryParse(item.Item_create_date, out DateTime created) ? created : DateTime.Now;
                                itemNew.Expire_date = DateTime.TryParse(item.Item_expire_date, out DateTime createdTo) ? createdTo : DateTime.Now;
                                array[index] = itemNew;
                                index++;
                            }
                        }

                        package.Items  = array;
                        return await Task.FromResult(package.Items);
                    }
                }
            }

            return await Task.FromResult<MyItem[]>(null);
        }
        #endregion

        #region Insert a new 
        public async Task<ItemPackage> InsertAsync(int item_cate, string item_name, double item_price, string item_image, int item_quantity, string item_expire_date, double item_price_buy)
        {
            var result_post = await _httpRequests.ImagePostAsync(FORMATS.URL_POST_ITEMS_IMAGES, item_image, item_name);
            if (!result_post)
            {
                await Shell.Current.DisplayAlert("INFO","Something wrong wiht upload image to websocket try again later..", "OK");
                return await Task.FromResult(package);
            }

            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodInsert);
            MyItemRequest bodyData = new()
            {
                item_cate = item_cate,
                item_name = item_name,
                item_price = item_price,
                item_image = string.Format("{0}{1}", item_name, Path.GetExtension(item_image)),
                item_quantity = item_quantity,
                item_expire_date = item_expire_date,
                item_price_buy = item_price_buy
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            var result = await _httpRequests.PutAsync(url, jsonData);
            if (result.Item1)
            {
                if (result.Item2.Contains("status") || result.Item2.Contains("message")  || result.Item2.Contains("error"))
                {
                    package = JsonConvert.DeserializeObject<ItemPackage>(result.Item2);
                    return await Task.FromResult(package);
                }
            }

            return await Task.FromResult(package);
        }
        #endregion

        #region Delete Item
        public async Task<bool> Del(int id)
        {
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodDel);
            MyItemRequest bodyData = new MyItemRequest()
            {
                Id = id
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            var result = await _httpRequests.PutAsync(url, jsonData);
            if (result.Item1)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
        #endregion
    }
}
