using CashY.Core;
using CashY.Model;
using CashY.Pop;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
namespace CashY.Services
{
    /// <summary>
    /// injection login service
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// START TO LOGIN IN DATABASE SERVER API WEB SOCKET
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<(bool, string)> LoginRequestHttp(string username, string password);
    }

    /// <summary>
    /// LOGIN SERVICE CLASS.
    /// </summary>
    public class LoginService : ILoginService
    {
        // API KEY CLASS & METHOD
        const string ClassName = "Account";
        const string MethodName = "Login";

        // JSON MODEL ACCOUNT PACKAGE
        public AccountPackage package { get; set; }

        // SERVICES PROVIDER
        private IConnection connection { get;  set; }
        private IMyHttpRequests httpRequests { get;  set; }

        // CTOR
        public LoginService(IConnection connection, IMyHttpRequests httpRequests)
        {
            this.connection = connection;
            this.httpRequests = httpRequests;
        }

        /// <summary>
        /// START TO LOGIN IN DATABASE SERVER API WEB SOCKET
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<(bool, string)> LoginRequestHttp(string username, string password)
        {
            try
            {
                // Get account information from api
                var items = await GetAccount(username, password);

                // get respone items from task async
                AccountPackage accountPackage = items.Item1;
                ErrorAccountFlag ErrorFlag = items.Item2;

                // check flag is no ok status
                if (ErrorFlag != ErrorAccountFlag.OK)
                {
                    return (false, FORMATS.MSG_DISCONNECTED);
                }
                else
                {
                    // Check if nullable json model
                    if (accountPackage != null)
                    {
                        // Check respone error message from api.
                        if (!string.IsNullOrEmpty(accountPackage.error))
                        {
                            return (false, accountPackage.error);
                        }
                        else
                        {
                            return (true, accountPackage.message);
                        }
                    }
                    else
                    {
                        // send message popup
                        return (false, FORMATS.MSG_DISCONNECTED);
                    }
                }
            }
            catch (Exception ex) 
            {
                await Console.Out.WriteLineAsync($"Error : {ex.Message}");
            }


            return (false, "Something wrong happend try again later!");
        }

        /// <summary>
        /// Get Account API WEB SOCKET
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<(AccountPackage, ErrorAccountFlag)> GetAccount(string username, string password)
        {
            // string format for api path socket
            string url = string.Format(FORMATS.URL_HOME, ClassName, MethodName);

            // check if not connection to networking.
            if (!connection.IsConnected)
            {
                return await Task.FromResult<(AccountPackage, ErrorAccountFlag)>((null, ErrorAccountFlag.isDisconnected));
            }

            AccountRequest bodyData = new()
            {
                username = username,
                password = password,
            };

            string jsonData = JsonConvert.SerializeObject(bodyData);
            var result = await httpRequests.PutAsync(url, jsonData);

            if (result.Item1)
            {
                if (result.Item2.Contains("status") || result.Item2.Contains("message") || result.Item2.Contains("error"))
                {
                    package = JsonConvert.DeserializeObject<AccountPackage>(result.Item2);
                    return await Task.FromResult((package, ErrorAccountFlag.OK));
                }
            }

            return await Task.FromResult((package, ErrorAccountFlag.OK));
        }
    }


    public enum ErrorAccountFlag
    {
        isDisconnected,
        Error,
        DidNotRespone,
        OK
    }
}
