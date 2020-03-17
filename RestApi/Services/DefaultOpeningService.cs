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
    public class DefaultOpeningService : IOpeningService
    {
        private readonly HotelApiDbContext _context;
        private readonly IDateLogicService _dateLogicService;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultOpeningService(
            HotelApiDbContext context,
            IDateLogicService dateLogicService,
            IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            _dateLogicService = dateLogicService;
            _mappingConfiguration = mappingConfiguration;
        }

        /// <summary>
        /// 
        /// "entity framework translates the linw expression in to sql query, so that it can execuete the whole thing on databse/server , instead of fetching the whole data to client side"
        /// 
        /// 'new System.Linq.SystemCore_EnumerableDebugView<RestApi.Models.BookingEntity>(k).Items' threw an exception of type 'System.InvalidOperationException'
        ///
        ///
        ///
        ///     The LINQ expression 'DbSet<BookingEntity>
        /// .LeftJoin(
        ///     outer: DbSet<RoomEntity>,
        ///     inner: b => EF.Property<Nullable<Guid>>(b, "RoomId"), 
        ///     outerKeySelector: r => EF.Property<Nullable<Guid>>(r, "Id"), 
        ///     innerKeySelector: (o, i) => new TransparentIdentifier<BookingEntity, RoomEntity>(
        ///         Outer = o, 
        ///         Inner = i
        ///     ))
        /// .Where(b => b.Inner.Id == __roomid_0 && ___dateLogicService_1.DoesConflict(
        ///     b: b.Outer,
        ///     start: __start_2,
        ///     end: __end_3))' could not be translated. Either rewrite the query in a form that can be translated, 
        ///     or switch to client evaluation explicitly by inserting a call to either AsEnumerable(), AsAsyncEnumerable(), ToList(), or ToListAsync(). 
        ///     See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.
        /// 
        /// The above is failing because it is not supported in .net core 3.0 and above
        /// 
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BookingRange>> GetConflictingSlots(
            Guid roomid,
            DateTimeOffset start, 
            DateTimeOffset end)
        {

            //https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            //https://stackoverflow.com/questions/1578778/using-iqueryable-with-linq
            var result = _context.Bookings.Where(b => (b.Room.Id == roomid)).AsEnumerable().Where(b=> _dateLogicService.DoesConflict(b, start, end)).SelectMany(existing => _dateLogicService
                 .GetAllSlots(existing.StartAt, existing.EndAt));

            //_dateLogicService.DoesConflict(b, start, end))).Select(x=>x);

            /*var l = k

            .SelectMany(existing => _dateLogicService
                 .GetAllSlots(existing.StartAt, existing.EndAt));*/
            //.ToArrayAsync();

            // return await Task.FromResult(l.ToArray());

            return await Task.FromResult(result.ToArray());
        }

        public async Task<PagedResults<Opening>> GetOpeningsAsync(PagingOptions pagingOptions, SortOptions<Opening,OpeningEntity> sortOptions)
        {
            var rooms = await _context.Rooms.ToArrayAsync();

            var allOpenings = new List<OpeningEntity>();

            foreach (var room in rooms)
            {
                var allPossibleOpenings = _dateLogicService.GetAllSlots(
                        DateTimeOffset.UtcNow,
                        _dateLogicService.FurthestPossibleBooking(DateTimeOffset.UtcNow))
                    .ToArray();

                var conflictedSlots = await GetConflictingSlots(
                    room.Id,
                    allPossibleOpenings.First().StartAt,
                    allPossibleOpenings.Last().EndAt);

                //remove the slots that have conflicts and project
                var openings = allPossibleOpenings.
                    Except(conflictedSlots, new BookingRangeComparer())
                    .Select(slot => new OpeningEntity
                    {
                        Roomid = room.Id,
                        Rate = room.Rate,
                        StartAt = slot.StartAt,
                        EndAt = slot.EndAt
                    });
                    //.Select(model => _mapper.Map<Opening>(model));

                allOpenings.AddRange(openings);
            }

            IQueryable<OpeningEntity> pseudoQuery = allOpenings.AsQueryable();
            pseudoQuery = sortOptions.Apply(pseudoQuery);

            var pagedOpenings = pseudoQuery
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<Opening>(_mappingConfiguration)
                .ToArray();

            var size = pseudoQuery.Count();
           
            return new PagedResults<Opening> {
                Items = pagedOpenings,
                TotalSize = size
            }
            
            ;
        }
    }
}
