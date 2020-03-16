using AutoMapper;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{/// <summary>
/// //profile class in Automapper package is used to define how the entity objects are mapped to corresponding resource objects
/// </summary>
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //the properties with same name and same type wil be automatically mapped
            CreateMap<RoomEntity, Room>().ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src => Link.To(nameof(Controllers.RoomsController.GetRoomById), 
                new { roomId = src.Id })));


            CreateMap<OpeningEntity, Opening>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate/100m) )
                .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src=> src.StartAt.UtcDateTime))
                .ForMember(dest => dest.EndAt, opt=> opt.MapFrom(src => src.EndAt.UtcDateTime))
                .ForMember(dest => dest.Room, opt=> opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomById), 
                        new { roomId=src.Roomid})))
                ;


            CreateMap<BookingEntity, Booking>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total / 100m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.BookingsController.GetBookingById),
                        new { bookingId = src.Id })))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomById), 
                        new { roomId = src.Id })));

        }
    }
}
