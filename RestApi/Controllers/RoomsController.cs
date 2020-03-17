using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestApi.Models;
using RestApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("/[controller]")]// [controller] template will match the name of the controller excluding word controller i.e. "GetRooms"
    [ApiController]//indicates it is an api controller , this does some automatic model validations. if it is not present we have to start every method wiht
    // If(!ModelState.IsValid) return BadRequest; but now if we pass invalid value for limit parameter like 0 then we could see that the error message format is not that of the error filter 
    //we 

    /* ASP.NET Core 2.1 added the [ApiController] attribute, which among other things, automatically handles model validation errors by returning a
     * BadRequestObjectResult with ModelState passed in. In other words, if you decorate your controllers with that attribute, 
     * you no longer need to do the if (!ModelState.IsValid) check. Additionally, the functionality is also extensible. In Startup
     *     
     *       services.Configure<ApiBehaviorOptions>(options =>
            {
                // var errorResponse = 
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiError(context.ModelState);

                    return new BadRequestObjectResult(errorResponse);
                };
            });
     */
    public class RoomsController:ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IOpeningService _openingService;
        private readonly PagingOptions _defaultPagingOptions;


        public RoomsController(IRoomService roomService,
            IOpeningService openingService,
            IOptions<PagingOptions> defaultPagingOptionsWrapper            
            )
        {
            _roomService = roomService;
            _openingService = openingService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }


        /*https://localhost:44328/rooms?orderBy=Rate DESC*/

        [HttpGet(Name =nameof(GetAllRooms))]//Name is route        
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Room>>> GetAllRooms([FromQuery]PagingOptions pagingOptions,
            [FromQuery] SortOptions<Room,RoomEntity> sortOptions)
        {

            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var rooms = await _roomService.GetRoomsAsync(pagingOptions, sortOptions);

            //var collection = new Collection<Room>
            //{
            //    Self = Link.ToCollection(nameof(GetAllRooms)),
            //    Value = rooms.ToArray(),
            //};

            var collection = PagedCollection<Room>.Create(
                Link.ToCollection(nameof(GetAllRooms)),
                rooms.Items.ToArray(),
                rooms.Items.Count(),
                pagingOptions
                );

            return collection;
        }

        // GET /rooms/openings
        [HttpGet("openings", Name = nameof(GetAllRoomOpenings))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<Opening>>> GetAllRoomOpenings([FromQuery]PagingOptions pagingOptions,
            [FromQuery] SortOptions<Opening,OpeningEntity> sortOptions            
            )//[]FromQuery tells that the pageoptions is taken from query string
        {

            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;


            var openings = await _openingService.GetOpeningsAsync(pagingOptions,sortOptions);

            var collection = PagedCollection<Opening>.Create(

                Link.ToCollection(nameof(GetAllRoomOpenings)),
                openings.Items.ToArray(),
                openings.TotalSize,
                pagingOptions

                );
            //{
            //    Self = Link.ToCollection(nameof(GetAllRoomOpenings)),
            //    Value = openings.Items.ToArray(),
            //    Size=openings.TotalSize,
            //    Offset=pagingOptions.Offset.Value,//If any of the value of offset or Limit(nullable) is not available api will throw an null referenece
            //    Limit=pagingOptions.Limit.Value
            //};

            return collection;
        }

        //Get /rooms/{roomId}
        [HttpGet("{roomId}", Name =nameof(GetRoomById))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
        {
            var room = await _roomService.GetRoomAsync(roomId);

            if(room == null)
            {
                return NotFound();
            }


            return room;
        }
    }
}
