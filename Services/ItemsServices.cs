using CashY.Core;
using CashY.Model.Items;
using CashY.Model.Items.Items;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CashY.Services
{
    public interface IItemsServices
    {
        int currentCount { get; set; }
        Task<(bool, MyItem)> InsertAsync(int item_cate, string item_name, double item_price, string item_image, int item_quantity, string item_expire_date, double item_price_buy);
        Task<MyItem[]> GetItemsAsync();
        Task<MyItem[]> GetNext();
        Task<MyItem[]> GetBack();
        Task<bool> Del(int id);
    }

    public class ItemsServices : IItemsServices
    {

        private readonly IServiceProvider serviceProvider;
        private readonly IMyHttpRequests httpRequests;
        private readonly IDatabaseServices databaseServices;
        private readonly ILoadData load;
        private readonly ILoggerServices loggerServices;

        private readonly int limited = 1225;
        private int currentIndex = 0;
        public int currentCount { get => currentIndex; set => currentIndex = value; }

        public ItemsServices(IServiceProvider _serviceProvider, IMyHttpRequests _httpRequests, IDatabaseServices _databaseServices, ILoadData _load, ILoggerServices loggerServices)
        {
            serviceProvider  = _serviceProvider;
            httpRequests     = _httpRequests;
            databaseServices = _databaseServices;
            load             = _load;
            this.loggerServices=loggerServices;
        }

        #region Get All
        public async Task<MyItem[]> GetItemsAsync()
        {
            await databaseServices.GetItems();
            return await Task.FromResult(databaseServices.items.ToArray());
        }

        public async Task<MyItem[]> GetNext()
        {
            try
            {
                await databaseServices.GetItems();
                MyItem[] array = databaseServices.items.Skip(limited * currentCount).Take(limited).ToArray();

                if (array.Length == 0)
                    return null;
                currentCount++;


                for (int i = 0; i < array.Length; i++)
                    await array[i].reloadImage();

                return await Task.FromResult(array);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public async Task<MyItem[]> GetBack()
        {
            await databaseServices.GetItems();
            // Calculate the ending index for the previous items
            int endIndex = (currentCount * limited) - limited;

            // Check if the endIndex is negative and set it to 0 if necessary
            if (endIndex < 0)
            {
                endIndex = 0;
            }

            // Take the previous 'limited' items starting from the calculated endIndex.
            MyItem[] array = databaseServices.items.Skip(endIndex).Take(limited).ToArray();
            if (array.Length == 0)
                return null;

            for (int i = 0; i < array.Length; i++)
                await array[i].reloadImage();

            // Update the currentCount for the next retrieval
            currentCount--;
            return await Task.FromResult(array);
        }
        #endregion


        #region Insert a new 
        public async Task<(bool, MyItem)> InsertAsync(int item_cate, string item_name, double item_price, string item_image, int item_quantity, string item_expire_date, double item_price_buy)
        {
            try
            {
                var result_post = await httpRequests.ImagePostAsync(FORMATS.URL_POST_ITEMS_IMAGES, item_image, item_name);
                if (!result_post)
                {
                    await Shell.Current.DisplayAlert("INFO", "Something wrong wiht upload image to websocket try again later..", "OK");
                    return (false, null);
                }

                MyItem bodyData = serviceProvider.GetService<MyItem>();
                bodyData.Item_cate = item_cate;
                bodyData.Item_name = item_name;
                bodyData.Item_price = item_price;
                bodyData.Item_image = string.Format("{0}{1}", item_name, Path.GetExtension(item_image));
                bodyData.Item_quantity = item_quantity;
                bodyData.Item_expire_date = item_expire_date;
                bodyData.Item_price_buy = item_price_buy;


                using (var connection = databaseServices.GetConnection())
                {
                    string query = "INSERT INTO items (item_cate, item_name, item_price, item_image, item_quantity, item_expire_date, item_price_buy, item_create_date) VALUES (@item_cate, @item_name, @item_price, @item_image, @item_quantity, @item_expire_date, @item_price_buy, NOW())";
                    var execute = await connection.ExecuteAsync(query, new { item_cate = bodyData.Item_cate, item_name = bodyData.Item_name, item_price = bodyData.Item_price, item_image = bodyData.Item_image, item_quantity = bodyData.Item_quantity, item_expire_date = bodyData.Item_expire_date, item_price_buy = bodyData.Item_price_buy });
                    if (execute > 0)
                    {
                        query = "SELECT * FROM items WHERE item_name=@item_name";
                        var result = await connection.QueryAsync<CategoryRequest>(query, new { item_name = bodyData.Item_name});

                        if (result != null && result.Count() > 0)
                        {
                            var first = result.First();
                            bodyData.Id = first.id;
                        }

                        databaseServices.items.Add(bodyData);
                        await load.ReloadingData();
                        await bodyData.reloadImage();
                        return await Task.FromResult((true, bodyData));
                    }
                }
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error Service Items :{ex}");
            }
            return (false, null);
        }
        #endregion

        #region Delete Item
        public async Task<bool> Del(int id)
        {

            using (var connection = databaseServices.GetConnection())
            {
                string query = "DELETE FROM items WHERE id =@Id";
                var execute = await connection.ExecuteAsync(query, new { Id = id });
                if (execute > 0)
                {
                    await load.DelItemImage(id);
                    var item = databaseServices.items.Where(x => x.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        await loggerServices.Insert($"Del item in item name {item.Item_name}", "1");
                        databaseServices.items.Remove(item);
                        return true;
                    }
                }

                return await Task.FromResult(false);
            }
        }
        #endregion
    }
}
