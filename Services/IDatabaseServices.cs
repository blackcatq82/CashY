using CashY.Model.Items;
using CashY.Model.Items.Items;
using CashY.Model.Items.Logger;
using CashY.Model.Items.Payment;
using Dapper;
using System.Data;
namespace CashY.Services
{
    public interface IDatabaseServices
    {
        List<Category> categories { get; set; }
        List<MyItem> items { get; set; }
        List<MyPayment> payments { get; set; }
        List<MyLogger> loggers { get; set; }

        IDbConnection GetConnection();

        Task GetCategorys();
        Task GetItems();
        Task GetPayments();
        Task GetLogger();

        Task Startup();
    }
    public class DatabaseServices : IDatabaseServices
    {
        readonly string _connectionString = @"server=gator2122.hostgator.com;database=shanpous_Cash;uid=shanpous_Blackcatq8;pwd=Xxionfeng22;";
        private IDbConnection _connection { get { return new MySql.Data.MySqlClient.MySqlConnection(_connectionString); } }


        private List<Category> _categories;
        public List<Category> categories { get => _categories; set => _categories = value; }


        private List<MyItem> _items;
        public List<MyItem> items { get => _items; set => _items = value; }


        private List<MyPayment> _payments;
        public List<MyPayment> payments { get => _payments; set => _payments = value; }


        private List<MyLogger> _loggers;
        public List<MyLogger> loggers { get => _loggers; set => _loggers = value; }


        public IDbConnection GetConnection()
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
            if(conn == null)
            {
                Console.WriteLine("Error Create connection mysql");
                return null;
            }

            if(conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error create connection open :{ex.Message}");
                }
            }
            return conn;
        }

        readonly IServiceProvider _serviceProvider;

        public DatabaseServices(IServiceProvider _serviceProvider)
        {
            this._serviceProvider = _serviceProvider; ;
            categories = new List<Category>();
            items = new List<MyItem>();
            payments = new List<MyPayment>();
            loggers = new List<MyLogger>();
        }
        public async Task Startup()
        {
            try
            {
                await GetCategorys();
                await GetItems();
                await GetPayments();
                await GetLogger();
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync($"error DatabaseServices data : {ex}");
            }
        }

        public async Task GetCategorys()
        {
            using(var connection = GetConnection())
            {
                categories = new List<Category>();
                string query = "SELECT * FROM cate ORDER BY id DESC";
                try
                {
                    var result = await connection.QueryAsync<CategoryRequest>(query);
                    if (result != null && result.Count() > 0)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            foreach (var category in result)
                            {
                                var cate = scope.ServiceProvider.GetService<Category>();
                                await cate.CopyTo(category);
                                categories.Add(cate);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    await Console.Out.WriteLineAsync("Query : " + ex.ToString());
                }
            }
        }
        public async Task GetItems()
        {
            using (var connection = GetConnection())
            {
                items = new List<MyItem>();
                string query = "SELECT * FROM items ORDER BY id DESC";
                var result = await connection.QueryAsync<MyItemRequest>(query);
                if (result != null && result.Count() > 0)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        foreach (var Myitem in result)
                        {
                            var item = scope.ServiceProvider.GetService<MyItem>();
                            await item.CopyTo(Myitem);
                            items.Add(item);
                        }
                    }
                }
            }
        }
        public async Task GetPayments()
        {
            using (var connection = GetConnection())
            {
                payments = new List<MyPayment>();
                string query = "SELECT * FROM payments ORDER BY id DESC";
                var result = await connection.QueryAsync<PaymentRequest>(query);
                if (result != null && result.Count() > 0)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        foreach (var paymentRequest in result)
                        {
                            var payment = scope.ServiceProvider.GetService<MyPayment>();
                            await payment.CopyTo(paymentRequest);
                            payments.Add(payment);
                        }
                    }
                }
            }
        }
        public async Task GetLogger()
        {
            using (var connection = GetConnection())
            {
                loggers = new List<MyLogger>();
                string query = "SELECT * FROM logger ORDER BY id DESC";
                var result = await connection.QueryAsync<LoggerRequest>(query);
                if (result != null && result.Count() > 0)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        foreach (var loggerRequest in result)
                        {
                            var logger = scope.ServiceProvider.GetService<MyLogger>();
                            await logger.CopyTo(loggerRequest);
                            loggers.Add(logger);
                        }
                    }
                }
            }
        }
    }
}
