using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class CategoryTrashDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedAgo => DeletedAt == null ? "No Data": $"{(DateTime.UtcNow - DeletedAt.Value).Days} days ago";
    }
}
