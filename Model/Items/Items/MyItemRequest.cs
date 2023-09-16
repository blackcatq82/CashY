namespace CashY.Model.Items.Items
{
    public class MyItemRequest
    {
        public int Id { get; set; }
        public int item_cate { get; set; }
        public string item_name { get; set; }
        public double item_price { get; set; }
        public string item_image { get; set; }
        public int item_quantity { get; set; }
        public string item_expire_date { get; set; }
        public string item_create_date { get; set; }
        public double item_price_buy { get; set; }
    }
}
