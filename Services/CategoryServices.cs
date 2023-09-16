using CashY.Core;
using CashY.Model.Items;
using Newtonsoft.Json;

namespace CashY.Services
{
    public interface ICategoryServices
    {
        CategoryPackage package { get; set; }
        Task<Category[]> GetCategoriesAsync();
        Task<bool> Del(int id);

        Task<CategoryPackage> InsertAsync(string cate_name, string cate_image);
    }
    public class CategoryServices : ICategoryServices
    {
        private string ClassName = "Category";
        private string MethodInsert = "Insert";
        private string MethodGetAll = "GetAll";
        private string MethodDel = "Del";

        public CategoryPackage package { get => _package; set => _package = value; }
        private CategoryPackage _package;

        private IServiceProvider _serviceProvider;
        private IMyHttpRequests _httpRequests;
        public CategoryServices(IServiceProvider serviceProvider, IMyHttpRequests httpRequests)
        {
            _serviceProvider = serviceProvider;
            _httpRequests = httpRequests;

        }

        #region Category Reload & Get Items
        public async Task<Category[]> GetCategoriesAsync()
        {
            // Set format api call api class and method.
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodGetAll);
            CategoryRequest bodyData = new()
            {
                cate_name = "GetAll",
                cate_image = "GetAll",
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            var result = await _httpRequests.PutAsync(url, jsonData);

            if (result.Item1)
            {
                if (result.Item2.Contains("getall"))
                {
                    if (result.Item2.Contains("status") || result.Item2.Contains("message") || result.Item2.Contains("error"))
                    {
                        package = JsonConvert.DeserializeObject<CategoryPackage>(result.Item2);

                        Category[] array = new Category[package.Categorys.Length];
                        int index = 0;  
                        foreach (var category in package.Categorys)
                        {
                            using(var scope = _serviceProvider.CreateScope())
                            {
                                var cateNew = scope.ServiceProvider.GetRequiredService<Category>();
                                cateNew.Cate_name = category.cate_name;
                                cateNew.Cate_image = category.cate_image;
                                cateNew.Id = category.id;
                                cateNew.Create_date = category.Create_date;
                                cateNew.Create_datee = DateTime.TryParse(category.create_date, out DateTime created) ? created : DateTime.Now;
                                array[index] = cateNew;
                                array[index].SetDateTime();
                                index++;
                            }
                        }
                        package.Categorys  = array;
                        return await Task.FromResult(package.Categorys);
                    }
                }
            }

            return await Task.FromResult<Category[]>(null);
        }
        #endregion

        #region Delete Item
        public async Task<bool> Del(int id)
        {
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodDel);
            CategoryRequest bodyData = new CategoryRequest()
            {
                id = id
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

        #region Insert a new 
        public async Task<CategoryPackage> InsertAsync(string cate_name, string cate_image)
        {
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodInsert);
            CategoryRequest bodyData = new()
            {
                cate_name = cate_name,
                cate_image = cate_image,
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            var result = await _httpRequests.PutAsync(url, jsonData);
            if (result.Item1)
            {
                if (result.Item2.Contains("status") || result.Item2.Contains("message") || result.Item2.Contains("error"))
                {
                    package = JsonConvert.DeserializeObject<CategoryPackage>(result.Item2);
                    return await Task.FromResult(package);
                }
            }

            return await Task.FromResult(package);
        }
        #endregion
    }
}
