using AuthService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class UpdateAddressDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public AddressType AddressType { get; set; }
        [Phone]
        [MaxLength(20)]
        public string? ContactPhoneNumber { get; set; }
        [Required]
        [MaxLength(250)]
        public string AddressLine1 { get; set; } = null!;
        [MaxLength(250)]
        public string? AddressLine2 { get; set; }
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string State { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}