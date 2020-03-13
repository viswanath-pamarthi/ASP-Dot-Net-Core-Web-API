using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using RestApi.Services;
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
        private readonly IRoomService _roomService;


        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }



        [HttpGet(Name =nameof(GetRooms))]//Name is route
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public IActionResult GetRooms()
        {
            throw new NotImplementedException();
        }
        //Get /rooms/{roomId}
        [HttpGet("{roomId}", Name =nameof(GetRoomById))]
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
