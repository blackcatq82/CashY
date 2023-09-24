using CashY.Model.Items.Payment;
using Dapper;
namespace CashY.Services
{
    public interface IPaymentServices
    {
        Task<MyPayment[]> GetPaymentsAsync();
        Task<bool> Del(int id);
    }

    public class PaymentServices : IPaymentServices
    {
        private readonly IDatabaseServices databaseServices;
        private readonly ILoggerServices loggerServices;
        public PaymentServices(IDatabaseServices _databaseServices, ILoggerServices loggerServices)
        {
            databaseServices = _databaseServices;
            this.loggerServices=loggerServices;
        }

        #region Get All
        public async Task<MyPayment[]> GetPaymentsAsync()
        {
            await databaseServices.GetPayments();
            return await Task.FromResult(databaseServices.payments.ToArray());
        }
        #endregion

        #region Delete Item
        public async Task<bool> Del(int id)
        {
            using (var connection = databaseServices.GetConnection())
            {
                string query = "DELETE FROM payments WHERE id =@Id";
                var execute = await connection.ExecuteAsync(query, new { Id = id });
                if (execute > 0)
                {
                    var item = databaseServices.payments.Where(x => x.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        await loggerServices.Insert($"Del payment number {item.Id}", "1");
                        databaseServices.payments.Remove(item);
                    }
                }

                return await Task.FromResult(false);
            }
        }
        #endregion
    }
}
