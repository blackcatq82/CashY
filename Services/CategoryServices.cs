using CashY.Core;
using CashY.Model.Items;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CashY.Services
{
    public interface ICategoryServices
    {
        Task<Category[]> GetCategoriesAsync();
        Task<bool> Del(int id);

        Task<(bool, Category)> InsertAsync(string cate_name, string cate_image);
    }
    public class CategoryServices : ICategoryServices
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMyHttpRequests _httpRequests;
        private readonly IDatabaseServices databaseServices;
        private readonly ILoadData load;
        private readonly ILoggerServices logger;
        public CategoryServices(IServiceProvider serviceProvider, IMyHttpRequests httpRequests, IDatabaseServices _databaseServices, ILoadData load, ILoggerServices logger)
        {
            _serviceProvider = serviceProvider;
            _httpRequests = httpRequests;
            databaseServices = _databaseServices;
            this.load=load;
            this.logger = logger;

        }

        #region Category Reload & Get Items
        public async Task<Category[]> GetCategoriesAsync()
        {
            await databaseServices.GetCategorys();
            return await Task.FromResult(databaseServices.categories.ToArray());
        }
        #endregion

        #region Delete Item
        public async Task<bool> Del(int id)
        {
            using(var connection = databaseServices.GetConnection())
            {
                string query = "DELETE FROM cate WHERE id =@Id";
                var execute = await connection.ExecuteAsync(query, new {Id = id});
                if(execute > 0)
                {
                    await load.DelCateImage(id);
                    var item = databaseServices.categories.Where(x => x.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        await logger.Insert($"Del item in cate name {item.Cate_name}", "1");
                        databaseServices.categories.Remove(item);
                        return true;
                    }
                }

                return await Task.FromResult(false);
            }
        }
        #endregion

        #region Insert a new 
        public async Task<(bool, Category)> InsertAsync(string cate_name, string cate_image)
        {
            try
            {
                var result_post = await _httpRequests.ImagePostAsync(FORMATS.URL_POST_IMAGES, cate_image, cate_name);
                if (!result_post)
                {
                    await Shell.Current.DisplayAlert("INFO", "Something wrong wiht upload image to websocket try again later..", "OK");
                    return (false, null);
                }


                cate_image = string.Format("{0}{1}", cate_name, Path.GetExtension(cate_image));

                Category bodyData = _serviceProvider.GetService<Category>();
                bodyData.Cate_name = cate_name;
                bodyData.Cate_image = cate_image;

                using (var connection = databaseServices.GetConnection())
                {
                    string query = "INSERT INTO cate(cate_name, cate_image, create_date) VALUES (@cate_name, @cate_image, NOW())";
                    var execute = await connection.ExecuteAsync(query, new { cate_name = bodyData.Cate_name, cate_image = bodyData.Cate_image });
                    if (execute > 0)
                    {
                        query = "SELECT * FROM cate WHERE cate_name=@cate_name AND cate_image=@cate_image";
                        var result = await connection.QueryAsync<CategoryRequest>(query, new {cate_image = bodyData.Cate_image, cate_name = bodyData.Cate_name});
                        if(result != null && result.Count() > 0)
                        {
                            var first = result.First();
                            bodyData.Id = first.id;
                        }
                        await databaseServices.GetCategorys();
                        await load.ReloadingData();
                        await bodyData.ReloadImage();
                        return await Task.FromResult((true, bodyData));
                    }
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error Service Cates :{ex}");
            }
            return (false, null);
        }
        #endregion
    }
}
