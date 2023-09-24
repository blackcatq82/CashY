namespace CashY.Model.Items.Logger
{
    public class LoggerRequest
    {
        public int id { get; set; }
        public string message { get; set; }
        public string level { get; set; }
        public DateTime date_create { get; set; }
    }
}
