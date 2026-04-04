using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs
{
    public class UpdateCartDto
    {
        [Required]
        public int CartId { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
