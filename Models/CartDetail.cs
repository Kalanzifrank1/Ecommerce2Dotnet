using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce2.Models
{
    [Table("CartDetail")]
    public class CartDetail
    { 
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        [Required]
        public string? ImageUrl { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int Quanatity { get; set; }
    }
}
