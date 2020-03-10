using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class InfoController:ControllerBase
    {
        private readonly HotelInfo _hotelInfo;

        public InfoController(IOptions<HotelInfo> hotelInfoWrapper)
        {
            _hotelInfo = hotelInfoWrapper.Value;
        }


        [HttpGet(Name =nameof(Getinfo))]
        [ProducesResponseType(200)]
        public ActionResult<HotelInfo> Getinfo()
        {
            _hotelInfo.Href = Url.Link(nameof(Getinfo), null);

            return _hotelInfo;

        }
    }
}
