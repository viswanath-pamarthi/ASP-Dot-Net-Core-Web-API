using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //TODO: authorization

        public async Task<ActionResult<Booking>> GetBookingById(Guid bookingId)
        {
            var booking = await _bookingService.GetBookingAsync(bookingId);

            if (booking == null) return NotFound();

            return booking;
        }


    }
}
