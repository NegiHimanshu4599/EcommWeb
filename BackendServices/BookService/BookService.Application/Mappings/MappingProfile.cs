using AutoMapper;
using BookService.Application.DTOs;
using BookService.Domain.Entities.Book;
using BookService.Domain.Entities.Category;
using BookService.Domain.Entities.CoverType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Application.Mappings
{
    public class MappingProfile:Profile 
    {
        public MappingProfile()
        {
            CreateMap<Book, TrashBookDto>();
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CoverTypeId, opt => opt.MapFrom(src => src.CoverTypeId));
            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CoverTypeId, opt => opt.MapFrom(src => src.CoverTypeId)); ;
            //CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src =>src.ParentCategory != null? src.ParentCategory.Name: null))
                .ForMember(dest => dest.DisplayName,opt => opt.MapFrom(src => src.ParentCategory != null? src.ParentCategory.Name + " > " + src.Name: src.Name))
                .ForMember(dest => dest.IsParentCategory,opt => opt.MapFrom(src => src.ParentCategoryId == null));
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.ParentCategory,opt => opt.Ignore())
                .ForMember(dest => dest.Books,opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted,opt => opt.Ignore());
            CreateMap<CoverType, CoverTypeDto>().ReverseMap();
            CreateMap<Book, BookListDto>()
                .ForMember(dest => dest.DeletedAt,opt => opt.MapFrom(src => src.DeletedAt))
                .ForMember(dest => dest.IsDeleted,opt => opt.MapFrom(src => src.IsDeleted));
            CreateMap<Book, BookDetailDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CoverTypeId, opt => opt.MapFrom(src => src.CoverTypeId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CoverTypeName, opt => opt.MapFrom(src => src.CoverType.Name))
                .ForMember(dest => dest.DeletedAt,opt => opt.MapFrom(src => src.DeletedAt))
                .ForMember(dest => dest.RestoredAt,opt => opt.MapFrom(src => src.RestoredAt))
                .ForMember(dest => dest.IsDeleted,opt => opt.MapFrom(src => src.IsDeleted));
        }
    }
}