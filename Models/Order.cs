namespace Ecommerce2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public int OrderStatusId { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
