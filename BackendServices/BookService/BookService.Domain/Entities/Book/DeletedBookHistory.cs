using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BookService.Domain.Entities.Book
{
    public class DeletedBookHistory
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsPermanentDeleted { get; set; }
    }
}
