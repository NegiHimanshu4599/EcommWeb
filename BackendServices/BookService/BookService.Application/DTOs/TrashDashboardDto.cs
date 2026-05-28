using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class TrashDashboardDto
    {
        public int TotalDeletedBooks { get; set; }
        public int RestoredBooks { get; set; }
        public int PermanentlyDeletedBooks { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}