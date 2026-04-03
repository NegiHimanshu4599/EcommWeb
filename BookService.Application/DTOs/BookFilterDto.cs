using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class BookFilterDto
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public int? CoverTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
