using System.ComponentModel.DataAnnotations;
using Ecommerce2.Constants;

namespace Ecommerce2.Models.DTOs
{
    public class ChechoutModel
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        [MaxLength(30)]
        public string Address { get; set; }
    }
}
