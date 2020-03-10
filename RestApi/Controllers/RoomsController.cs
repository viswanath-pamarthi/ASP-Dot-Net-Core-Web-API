using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("/[controller]")]// [controller] template will match the name of the controller excluding word controller i.e. "GetRooms"
    [ApiController]//indicates it is an api controller 
    public class RoomsController:ControllerBase
    {
        private readonly HotelApiDbContext _context;


        public RoomsController(HotelApiDbContext hotelApiDbContext)
        {
            _context = hotelApiDbContext;
        }



        [HttpGet(Name =nameof(GetRooms))]//Name is route
        public IActionResult GetRooms()
        {
            throw new NotImplementedException();
        }
        //Get /rooms/{roomId}
        [HttpGet("{roomId}", Name =nameof(GetRoomById))]
        public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
        {
            var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == roomId);

            if(entity==null)
            {
                return NotFound();
            }


            var resource = new Room
            {
                Href = Url.Link(nameof(GetRoomById), new { roomId = entity.Id }),
                Name = entity.Name,
                Rate = entity.Rate / 100.0m
            };

            return resource;
        }
    }
}
