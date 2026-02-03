using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce2.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }
        [Required, MaxLength(40)]
        public string? AuthorName { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public double Price { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public List<CartDetail>? CartDetails { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }

        [NotMapped]
        public string GenreName { get; set; }
    }
}
