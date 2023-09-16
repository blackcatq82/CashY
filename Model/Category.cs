using CashY.Core;
using System.Net;
using Newtonsoft.Json;
using CashY.Model.Items;
using CashY.Services;

namespace CashY.Model
{
    public static class CategoryHelper
    {
        const string ClassName = "Category";
        const string MethodInsert = "Insert";
        const string MethodUpdate = "Update";
        const string MethodGetAll = "GetAll";
        const string MethodDel = "Del";

        private static CookieContainer Cookie = new();
        public static CategoryPackage package { get; set; }


        public static async Task<CategoryPackage> Insert(string cate_name, string cate_image)
        {
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodInsert);

            // check if not connection to networking.
            //if (!Connection.IsConnected)
            //    Core.PopUp.CreatePopUp?.Invoke(FORMATS.MSG_DISCONNECTED, FORMATS.CLOSE);

            Cookie = new();
            CategoryRequest bodyData = new()
            {
                cate_name = cate_name,
                cate_image = cate_image,
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            string html = "";
            var result = false; //await MyHttpRequests.Put(url, Cookie, html, jsonData);
            if (result)
            {
                if (html.Contains("status") || html.Contains("message") || html.Contains("error"))
                {
                    package = JsonConvert.DeserializeObject<CategoryPackage>(html);
                    return await Task.FromResult(package);
                }
            }

            return await Task.FromResult(package);
        }

    }
}
