namespace Models
{
    public class Sale
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public DateTime SaleDate { get; set; } // Добавьте, если есть в БД
        public decimal TotalPrice { get; set; } // Добавьте, если нужно
    }
}
 