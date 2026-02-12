using System.ComponentModel.DataAnnotations;

namespace Ecommerce2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public int OrderStatusId { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }
        [Required]
        public string? MobileNuumber { get; set;  }
        [Required]
        [MaxLength(20)]
        public string? PaymentMethod { get; set; }
        [Required]
        public string? Address { get; set; }
        public bool IsPaid { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
