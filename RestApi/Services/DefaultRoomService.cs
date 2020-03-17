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


        public async Task<PagedResults<Room>> GetRoomsAsync(PagingOptions pagingOptions, 
            SortOptions<Room, RoomEntity> sortOptions)
        {
            //pull all rooms out of context and map each one of it to room resource - instead of duplicating code in GetRoomAsync here - we can use queryableExtentions in automapper to project all at once

            IQueryable<RoomEntity> query= _context.Rooms;


            //apply sort options to query before it goes and hits the database.
            //Apply method is used to update this query  with any additional stuff we need to add sorting to the query before it goes to the database
            query = sortOptions.Apply(query);
            


            var size = await query.CountAsync();

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<Room>(_mappingConfiguration)//provide mapping configuration to ProjectTo, for which we need to inject IConfigurationProvider from AutoMapper
                .ToArrayAsync();

            return new PagedResults<Room>
            {
                Items=items,
                TotalSize=size
            };

        }
    }
}
