using System.ComponentModel.DataAnnotations;

namespace Ecomm.UI.Models.AuthDtos
{
    public class UpdateUserDto
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
