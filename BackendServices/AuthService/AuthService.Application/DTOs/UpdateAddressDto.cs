using AuthService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class UpdateAddressDto
    {
        public int Id { get; set; }
        public AddressType AddressType { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}