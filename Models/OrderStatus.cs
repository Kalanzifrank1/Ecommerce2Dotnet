using System.ComponentModel.DataAnnotations;

namespace Ecommerce2.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        [Required]
        public int StatusId { get; set; }
        [Required]
        [MaxLength(40)]
        public string? StatusName { get; set; }
    }
}
