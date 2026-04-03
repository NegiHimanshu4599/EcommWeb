using BookService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.DTOs
{
    public class CoverTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
