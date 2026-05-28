using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Domain.Entities.CoverType
{
    public class RestoredCoverTypeHistory
    {
        public int Id { get; set; }
        public int CoverTypeId { get; set; }
        public string Name { get; set; }
        public DateTime RestoredAt { get; set; }
    }
}
