namespace Models
{
    public class Availability
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } // Поле "Availability" в БД
    }
}