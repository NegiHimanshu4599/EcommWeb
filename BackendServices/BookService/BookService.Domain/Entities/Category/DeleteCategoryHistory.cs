using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Domain.Entities.Category
{
    public class DeletedCategoryHistory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsPermanentDeleted { get; set; }
    }
}
