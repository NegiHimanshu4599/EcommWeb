using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Domain.Entities.CoverType
{
    public class CoverType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book.Book> Books { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime? RestoredAt { get; set; }
    }
}
