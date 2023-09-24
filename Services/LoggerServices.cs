using CashY.Model.Items.Logger;
using Dapper;
namespace CashY.Services
{
    public interface ILoggerServices
    {
        Task<MyLogger[]> GetLoggersAsync();
        Task Insert(string message, string level);
    }


    public class LoggerServices : ILoggerServices
    {
        private readonly IDatabaseServices databaseServices;
        public LoggerServices(IDatabaseServices _databaseServices)
        {
            databaseServices = _databaseServices;
        }

        #region Get All
        public async Task<MyLogger[]> GetLoggersAsync()
        {
            await databaseServices.GetLogger();
            return await Task.FromResult(databaseServices.loggers.ToArray());
        }

        public async Task Insert(string message, string level)
        {
            using(var connection = databaseServices.GetConnection())
            {
                string query = "INSERT INTO logger(message, level, date_create) VALUES (@message, @level, NOW())";
                await connection.ExecuteAsync(query, new {message = message, level = level});
            }
        }
        #endregion
    }
}
