using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using BookService.Application.DTOs;
using BookService.Domain.Entities;

namespace BookService.Application.Mappings
{
    public class MappingProfile:Profile 
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CoverTypeId, opt => opt.MapFrom(src => src.CoverTypeId));
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CoverType, CoverTypeDto>().ReverseMap();
        }
    }
}
