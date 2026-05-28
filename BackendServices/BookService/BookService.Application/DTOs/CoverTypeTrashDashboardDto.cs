using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class CoverTypeTrashDashboardDto
    {
        public int TotalDeletedCoverTypes { get; set; }
        public int RestoredCoverTypes { get; set; }
        public int PermanentlyDeletedCoverTypes { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}