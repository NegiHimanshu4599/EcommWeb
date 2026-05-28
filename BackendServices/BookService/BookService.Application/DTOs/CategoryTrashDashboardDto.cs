using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class CategoryTrashDashboardDto
    {
        public int TotalDeletedCategories { get; set; }
        public int RestoredCategories { get; set; }
        public int PermanentlyDeletedCategories { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}