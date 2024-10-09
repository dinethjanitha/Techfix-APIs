namespace techfixAPI.Models
{
    public class Order
    {
        public int orderid { get; set; }
        public string ordername { get; set; }
        public int itemId { get; set; }
        public int quantity { get; set; }
        public string orderDate { get; set; }
        public double total { get; set; }

    }
}
