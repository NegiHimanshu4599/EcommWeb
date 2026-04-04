using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Mappings
{
    public class MappingProfile:Profile 
    {
        public MappingProfile()
        {
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<CartDto, CartItem>().ReverseMap();
        }
    }
}
