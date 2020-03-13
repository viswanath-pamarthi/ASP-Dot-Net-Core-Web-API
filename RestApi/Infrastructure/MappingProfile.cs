using AutoMapper;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    public class MappingProfile:Profile//profile class in Automapper package is used to define how the entity objects are mapped to corresponding resource objects
    {
        public MappingProfile()
        {
            //the properties with same name and same type wil be automatically mapped
            CreateMap<RoomEntity, Room>().ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m));

        }
    }
}
