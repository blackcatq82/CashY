using CashY.Core;
using System.Net;
using Newtonsoft.Json;
using CashY.ViewModels;
using CashY.Services;

namespace CashY.Model
{
    public class AccountRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Account : BaseViewModel
    {
        private int _id;
        public int id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        private string _username;
        public string username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }
        private string _email;
        public string email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _token_key;
        public string token_key
        {
            get { return _token_key; }
            set { SetProperty(ref _token_key, value); }
        }

        private string _date_expire_key;
        public string date_expire_key
        {
            get { return _date_expire_key; }
            set { SetProperty(ref _date_expire_key, value); }
        }
    }

    public class AccountPackage : BaseViewModel
    {
        private int _status;
        public int status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private Account _account;
        [JsonProperty("data")]
        public Account account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }

        private string _message;
        public string message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _error;
        public string error
        {
            get { return _error; }
            set { SetProperty(ref _error, value); }
        }
    }


    public static class AccountHelper
    {
        const string ClassName = "Account";
        const string MethodName = "Login";
        private static CookieContainer Cookie = new ();
        public static AccountPackage package { get; set; }

        public static async Task<AccountPackage> GetAccount(string username, string password)
        {
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodName);

            //// check if not connection to networking.
            //if (!Connection.IsConnected)
            //    Core.PopUp.CreatePopUp?.Invoke(FORMATS.MSG_DISCONNECTED, FORMATS.CLOSE);

            Cookie = new();
            AccountRequest bodyData = new ()
            {
                username = username,
                password = password,
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            string html = "";
            var result = false;// await MyHttpRequests.Put(url, Cookie, html, jsonData);
            if (result)
            {
                if (html.Contains("status") || html.Contains("message") || html.Contains("error"))
                {
                    package = JsonConvert.DeserializeObject<AccountPackage>(html);
                    return await Task.FromResult(package);
                }
            }

            return await Task.FromResult(package);
        }
    }
}
