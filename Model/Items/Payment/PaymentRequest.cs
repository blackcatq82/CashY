namespace CashY.Model.Items.Payment
{
    public class PaymentRequest
    {
        public int id { get; set; }
        public double price { get; set; }
        public DateTime date_create { get; set; }
        public string items_ids { get; set; }
        public string items_names { get; set; }
        public string items_count { get; set; }
    }
}
