using System.ComponentModel.DataAnnotations;

namespace Ecommerce2.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
