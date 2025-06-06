
namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public int GroupCode { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public int Type { get; set; } // 0 - штучный, 1 - развесной
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; } // Поле "Date" в БД
    }
}
