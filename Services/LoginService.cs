using CashY.Model;
using Dapper;
using System.Security.Cryptography;
using System.Text;

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

        // SERVICES PROVIDER
        private readonly IConnection connection;
        private readonly IDatabaseServices databaseServices;
        // CTOR
        public LoginService(IConnection connection, IDatabaseServices databaseServices)
        {
            this.connection = connection;
            this.databaseServices=databaseServices;
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

                if (items)
                    return (true, "Walcome again");
                else
                    return (false, "Check you information or network!");
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
        private async Task<bool> GetAccount(string username, string password)
        {
            // check if not connection to networking.
            if (!connection.IsConnected)
            {
                return false;
            }

            AccountRequest bodyData = new()
            {
                username = username,
                password = HashHelper.HashPassword(password),
            };
            try
            {

                using (var connection = databaseServices.GetConnection())
                {
                    string query = "SELECT * FROM users WHERE username=@username AND password=@password";
                    var execute = await connection.QueryAsync<AccountRequest>(query, new { username = bodyData.username, password = bodyData.password });
                    if (execute != null)
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex) 
            {
                _ = Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK!");
            }


            return false;
        }
    }


    public enum ErrorAccountFlag
    {
        isDisconnected,
        Error,
        DidNotRespone,
        OK
    }


    public static class HashHelper
    {
        public static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // Convert to hexadecimal format
                }

                return sb.ToString().ToLower();
            }
        }
    }
}
