using BookService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class UpdateBookDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int CoverTypeId { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
    }
}
