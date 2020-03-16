using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public class DefaultRoomService : IRoomService
    {
        private readonly HotelApiDbContext _context;
        //private readonly IMapper _mapper;//We don't need to inject both IMapper and Iconfiguration , we can remove I mapper
        private readonly IConfigurationProvider _mappingConfiguration;
        
        //, IMapper mapper
        public DefaultRoomService(HotelApiDbContext context, IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            //_mapper = mapper;
            _mappingConfiguration = mappingConfiguration;
        }

        public  async Task<Room> GetRoomAsync(Guid id)
        {
            var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }


            //var resource = new Room
            //{
            //    Href = null,//Url.Link(nameof(GetRoomById), new { roomId = entity.Id }),
            //    Name = entity.Name,
            //    Rate = entity.Rate / 100.0m
            //};

            var mapper = _mappingConfiguration.CreateMapper();

            //instead of above manual mapping of properties, let us use Automapper
            return mapper.Map<Room>(entity);
        }


        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            //pull all rooms out of context and map each one of it to room resource - instead of duplicating code in GetRoomAsync here - we can use queryableExtentions in automapper to project all at once

            var query = _context.Rooms
                .ProjectTo<Room>(_mappingConfiguration);//provice mapping configuration to ProjectTo, for which we need to inject IConfigurationProvider from AutoMapper

            return await query.ToArrayAsync();
        }
    }
}
